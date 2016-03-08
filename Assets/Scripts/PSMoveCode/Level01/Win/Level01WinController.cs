using UnityEngine;
using System.Collections;
using MoveServerNS;

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(Rigidbody))]
public class Level01WinController : MonoBehaviour, MotionController {
    //Windows Server
    public WinMoveServer moveServer;
    [Range(1, 6)]
    public int controller;

    //Player Movement
    public float speed = 6f;
    Vector3 movement;
    Animator anim;
    Rigidbody playerRigidbody;
    //int floorMask;
    //float cameraRayLength = 100f;

    //Walk
    private Vector3 initialPos;
    private Quaternion initialRot;
    private bool setInitial = false;
    private const float SPEED = 0.1f;

	// Use this for initialization
	void Start () {
        //default 1 controller
        controller = 1;
        anim = GetComponent<Animator>();
        playerRigidbody = GetComponent<Rigidbody>();
	}
	
	// Update is called once per frame
	public void FixedUpdate () {
        WinMoveController move = moveServer.getController(controller - 1);

        if (move != null) {
            //=================Allow player to move===========//
            //Move coordinates
            float moveX = move.getPositionSmooth().x;
            float moveY = move.getPositionSmooth().y;
            float moveZ = move.getPositionSmooth().z;

            Quaternion moveAngle = move.getQuaternion();

            RaycastHit hit = new RaycastHit();
            Debug.DrawLine(Camera.main.transform.position, transform.position, Color.green);
            if (Physics.Raycast(Camera.main.transform.position, transform.position, out hit)) {
                Debug.Log("Player hit " + hit.transform.gameObject.name);
            }

            //calculate delta position of move controller
            if (move.btnPressed(MoveButton.BTN_MOVE)) {
                //set initial position and rotation
                if (!setInitial)
                {
                    setInitialPos(new Vector3(moveX, moveY, moveZ), moveAngle);
                }
                else {
                    Vector3 deltaPos = new Vector3(moveX, moveY, moveZ) - initialPos;
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

    public void setInitialPos(Vector3 pos, Quaternion angle) {
        initialPos = pos;
        initialRot = angle;
        setInitial = true;
    }

    public void Move(Vector3 deltaPos) {
        playerRigidbody.MovePosition(transform.position + deltaPos);
    }

    public void Turning(Vector3 deltaPos) {
        Quaternion newAngle = Quaternion.LookRotation(deltaPos);
        playerRigidbody.MoveRotation(Quaternion.Lerp(transform.rotation, transform.rotation * newAngle, Time.deltaTime));
    }

    public void Animating(Vector3 deltaPos) {
        if (deltaPos.x > 0 || deltaPos.z > 0) {
            anim.SetBool("IsWalking", true);
        }
        anim.SetBool("IsWalking", false);
    }
}
