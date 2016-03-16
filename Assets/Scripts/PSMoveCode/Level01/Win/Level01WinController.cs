using UnityEngine;
using System.Collections;
using MoveServerNS;

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(Rigidbody))]
public class Level01WinController : MonoBehaviour, MotionController
{
    // Stores animation states for serialization.
    public enum PlayerState
    {
        Idle,
        Walking}

    ;
    //Windows Server
    public WinMoveServer moveServer;
    [Range(1, 6)]
    public int controller;
    public GameObject controlBall;
    public PlayerState state;
    public bool isControllable = false;

    //UI
    private Vector3 worldControllerInitPos;
    // TODO(sylvia): this is never used, commented out to surpress warning.
    //private Quaternion worldControllerInitRot;
    private Vector3 actualControllerPrevPos;
    private bool setUIInitial;

    //Player Movement
    public float speed = 6f;
    Vector3 movement;
    Animator anim;
    Rigidbody playerRigidbody;
    //int floorMask;
    //float cameraRayLength = 100f;

    //Walk
    private Vector3 initialPos;
    //TODO(sylvia): this is never used, and is causing warnings, commented out to supress warning.
    // private Quaternion initialRot;
    private bool setInitial = false;
    private const float SPEED = 0.1f;

    // Use this for initialization
    void Start()
    {
        //default 1 controller
        controller = 1;
        anim = GetComponent<Animator>();
        playerRigidbody = GetComponent<Rigidbody>();
        worldControllerInitPos = Camera.main.transform.position;
    }
	
    // Update is called once per frame
    public void FixedUpdate()
    {
        WinMoveController move = moveServer.getController(controller - 1);

        if (move != null && isControllable)
        {
            //Move coordinates
            float moveX = move.getPositionSmooth().x;
            float moveY = move.getPositionSmooth().y;
            float moveZ = move.getPositionSmooth().z;
            Vector3 actualControllerCurrPos = new Vector3(moveX, moveY, moveZ);

            Quaternion moveAngle = move.getQuaternion();

            //=================UI System======================//
            if (!setUIInitial)
            {
                //setInitialPos(actualControllerCurrPos, moveAngle, worldControllerInitPos, worldControllerInitRot, setUIInitial);
                worldControllerInitPos = Camera.main.transform.position + Camera.main.transform.forward;
                // TODO(sylvia):Never used: commented out to surpress warning.
                // worldControllerInitRot = Quaternion.identity;
                setUIInitial = true;
                actualControllerPrevPos = actualControllerCurrPos;
            }
            else
            {
                Vector3 delta = actualControllerCurrPos - actualControllerPrevPos;
                worldControllerInitPos += delta * 10;
                controlBall.transform.position = worldControllerInitPos;
                //actualControllerPrevPos = actualControllerCurrPos;
                //Debug.Log("Delta is " + delta.x + " " + delta.y + " " + delta.z);

                RaycastHit hit = new RaycastHit();
                Debug.DrawRay(Camera.main.transform.position, delta * 100, Color.green, Time.deltaTime, true);
                if (Physics.Raycast(Camera.main.transform.position, delta * 100, out hit, 100))
                {
                    Debug.Log("Player hit " + hit.transform.gameObject.name);
                }
            }
           
            
            //=================Allow player to move===========//
            //calculate delta position of move controller
            if (move.btnPressed(MoveButton.BTN_MOVE))
            {
                //set initial position and rotation
                if (!setInitial)
                {
                    setInitialPos(actualControllerCurrPos, moveAngle);
                }
                else
                {
                    Vector3 deltaPos = actualControllerCurrPos - initialPos;
                    deltaPos = deltaPos.normalized * SPEED;
                    deltaPos.y = 0;
                    deltaPos.z = -deltaPos.z;

                    //Turning, moving, animating, etc.
                    Move(deltaPos);
                    //Turning(deltaPos);
                    Animating(deltaPos);
                }
            }
        }
    }

    public void setInitialPos(Vector3 pos, Quaternion angle)
    {
        initialPos = pos;
        // TODO(sylvia): this is never used.        initialRot = angle;
        setInitial = true;
    }

    public void Move(Vector3 deltaPos)
    {
        playerRigidbody.MovePosition(transform.position + deltaPos);
    }

    public void Turning(Vector3 deltaPos)
    {
        Quaternion newAngle = Quaternion.LookRotation(deltaPos);
        playerRigidbody.MoveRotation(Quaternion.Lerp(transform.rotation, transform.rotation * newAngle, Time.deltaTime));
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
}
