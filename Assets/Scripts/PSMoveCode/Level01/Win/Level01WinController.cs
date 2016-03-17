﻿using UnityEngine;
using System.Collections;
using MoveServerNS;

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(Rigidbody))]
public class Level01WinController : MonoBehaviour, MotionController
{
    //Windows Server
    private WinMoveServer moveServer;

    [Range(1, 6)]
    public int controller;
    //public GameObject controlBall;
    public PlayerState state;
    public bool isControllable = false;

    //UI
    private Vector3 actualControllerPrevPos;
    private bool setUIInitial;
    private Transform moveCursor;
    private float SCALE = 15.0f;
    private Vector3 delta;

    //Player Movement
    public float speed = 6f;
    Animator anim;
    Rigidbody playerRigidbody;
    //int floorMask;
    //float cameraRayLength = 100f;

    //Walk
    private Vector3 initialPos;
    private const float SPEED = 0.1f;

    // Use this for initialization
    void Start()
    {
        //default 1 controller
        controller = 1;
        anim = GetComponent<Animator>();
        playerRigidbody = GetComponent<Rigidbody>();
        setUIInitial = false;
        moveServer = GameObject.FindObjectOfType<WinMoveServer>();
    }

    // Update is called once per frame
    public void FixedUpdate()
    {
        if (moveServer == null)
        {
            Debug.LogWarning("Please set up move server first");
            return;
        }
        WinMoveController move = moveServer.getController(controller - 1);

        if (move != null && isControllable)
        {
            //Move coordinates
            float moveX = move.getPositionSmooth().x;
            float moveY = move.getPositionSmooth().y;
            float moveZ = move.getPositionSmooth().z;
            Vector3 actualControllerCurrPos = new Vector3(moveX, moveY, moveZ);

            Quaternion moveAngle = move.getQuaternion();

            if (!setUIInitial)
            {
                setUIInitial = true;
                actualControllerPrevPos = actualControllerCurrPos;
            }
            else
            {
                delta = actualControllerCurrPos - actualControllerPrevPos;
                delta += transform.forward * delta.z;
                delta += transform.up * delta.y;
                delta += transform.right * delta.x;
                delta.z = -delta.z;
                
                RaycastHit hit = new RaycastHit();

                //If object is at the center, it is available
                if (Physics.SphereCast(Camera.main.ScreenPointToRay(new Vector3(Camera.main.pixelWidth / 2, Camera.main.pixelHeight / 2, 0)), 0, out hit, 100))
                {
                    //Debug.Log("Player hit " + hit.transform.gameObject.name);
                }
                actualControllerPrevPos = actualControllerCurrPos;
            }

            //=================UI System======================//
            /* Update move cursor */
            if (moveCursor == null)
            {
                moveCursor = Camera.main.transform.FindChild("MoveCursor");
                moveCursor.forward = Camera.main.transform.forward;
                moveCursor.up = Camera.main.transform.up;
                moveCursor.right = Camera.main.transform.right;
            }

            moveCursor.localPosition += delta * SCALE;

            /* Check move cursor to screen position */
            Vector3 cursorPosToScreen = Camera.main.WorldToScreenPoint(moveCursor.position);

            //Press move button on right top of screen will trigger menu
            if (cursorPosToScreen.x > Camera.main.pixelWidth * 4f / 5f &&
                cursorPosToScreen.y > Camera.main.pixelHeight * 4f / 5f)
            {
                if (move.btnOnRelease(MoveButton.BTN_MOVE))
                {
                    MenuUI menuUIScript = Transform.FindObjectOfType<MenuUI>();
                    menuUIScript.ToggleMenu();
                }
            }

            //=================Allow player to move===========//
            //calculate delta position of move controller
            else if (move.btnOnPress(MoveButton.BTN_MOVE))
            {
                setInitialPos(actualControllerCurrPos, moveAngle);
            }
            else if (move.btnPressed(MoveButton.BTN_MOVE))
            {
                //set initial position and rotation
                Vector3 deltaPos = actualControllerCurrPos - initialPos;
                deltaPos = deltaPos.normalized * SPEED;
                deltaPos.z = -deltaPos.z;

                //Turning, moving, animating, etc.
                if (move.triggerValue > 0)
                {
                    deltaPos.y = 0;
                    Move(deltaPos);
                }
                else
                {
                    Turning(deltaPos);
                }

                Animating(deltaPos);            
            }
        }
    }

    public void setInitialPos(Vector3 pos, Quaternion angle)
    {
        initialPos = pos;
    }

    public void Move(Vector3 deltaPos)
    {
        playerRigidbody.MovePosition(transform.position +
            transform.forward * deltaPos.z
            + transform.right * deltaPos.x +
            transform.up * deltaPos.y);
    }

    public void Turning(Vector3 deltaPos)
    {
        deltaPos = deltaPos.normalized;
        //Rotate camera around x axis, player around y axis
        if (Mathf.Abs(deltaPos.y) > Mathf.Abs(deltaPos.x))
        {
            Camera.main.transform.Rotate(-deltaPos.y, 0, 0);
        }
        else
        {
            Quaternion newAngle = Quaternion.LookRotation(new Vector3(deltaPos.x, 0, 0));
            playerRigidbody.MoveRotation(Quaternion.Lerp(transform.rotation, 
                    transform.rotation * newAngle, Time.deltaTime));
        }
    }

    public void Animating(Vector3 deltaPos)
    {
        float h = deltaPos.x;
        float v = deltaPos.z;
        // If either h or v is not 0, the player is walking. 
        bool isWalking = h > 0f || v > 0f;
        if (isWalking)
        {
            state = PlayerState.Walking;   
        }
        else
        {
            state = PlayerState.Idle;
        }
        SetAnimationState(state);
    }

    // Set triggers for animation.
    public void SetAnimationState(PlayerState state)
    {
        if (state == PlayerState.Walking)
        {
            anim.SetBool("IsWalking", true); 
        }
        else
        {
            anim.SetBool("IsWalking", false);
        }

    }

    public void SetMoveServer(WinMoveServer server)
    {
        moveServer = server;
    }
}
