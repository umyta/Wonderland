using UnityEngine;
using System.Collections;
using System.Collections.Generic;  //Dictionary

using MoveServerNS;                //Sony move server
[RequireComponent(typeof(MenuUI))]
[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(Rigidbody))]
public class Level01WinController : MonoBehaviour, MotionController
{
    public Camera camera;
    //Windows Server
    private WinMoveServer moveServer;

    [Range(1, 6)]
    public int controller;
    //public GameObject controlBall;
    public PlayerState state;
    public bool isControllable = false;

    //UI
    private Vector3 actualControllerPrevPos;
    private Vector3 delta;
    private Transform moveCursor;
    MenuUI menuUIScript;
    private bool UIActive;
    private bool setUIInitial;
    private float SCALE = 15.0f;

    //Tools
    Dictionary<int, GameObject> toolMap = new Dictionary<int, GameObject>();

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
        menuUIScript = GetComponent<MenuUI>();
        moveCursor = camera.transform.FindChild("MoveCursor");

        if (moveServer != null)
        {
            Debug.Log("Move server is setup for player " + PhotonNetwork.player.ID);
        }
    }


    void Awake()
    {
        // TODO(sainan): consider tag all tools as Tool instead of individual tags.
        // Considering that we may need to check type of classes to differentiate game logic
        // in clickDetector.
        menuUIScript = GetComponent<MenuUI>();
        GameObject[] tools = GameObject.FindGameObjectsWithTag("ResizeTool");
        foreach (GameObject obj in tools)
        {
            toolMap[obj.GetInstanceID()] = obj;
        }
        tools = GameObject.FindGameObjectsWithTag("SpringTool");
        foreach (GameObject obj in tools)
        {
            toolMap[obj.GetInstanceID()] = obj;
        }
        tools = GameObject.FindGameObjectsWithTag("MagnetTool");
        foreach (GameObject obj in tools)
        {
            toolMap[obj.GetInstanceID()] = obj;
        }
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

        if (camera == null)
        {
            camera = transform.GetComponentInChildren<Camera>();
        }

        if (move != null && isControllable)
        {
            //Move coordinates
            float moveX = move.getPositionSmooth().x;
            float moveY = move.getPositionSmooth().y;
            float moveZ = move.getPositionSmooth().z;
            Vector3 actualControllerCurrPos = new Vector3(moveX, moveY, moveZ);

            if (!setUIInitial)
            {
                setUIInitial = true;
                actualControllerPrevPos = actualControllerCurrPos;
            }
            else
            {
                delta = actualControllerCurrPos - actualControllerPrevPos;
                actualControllerPrevPos = actualControllerCurrPos;

                /* Update move cursor */

                delta += moveCursor.transform.forward * delta.z;
                delta += moveCursor.transform.up * delta.y;
                delta += moveCursor.transform.right * delta.x;
                delta.z = -delta.z;

                moveCursor.position += delta * SCALE;
            }

            GameObject mousePointedAt = HelperLibrary.WorldToScreenRaycast(moveCursor.position, camera, 1000);

            /* Hover */
            CheckHoverOverMenuItems();

            if (mousePointedAt != null && move.btnOnRelease(MoveButton.BTN_MOVE))
            {
                CheckMenuSelected(mousePointedAt);
                CheckToolClick(mousePointedAt);
            }

            /* Toggle open and close menu */
            if (move.btnOnRelease(MoveButton.BTN_MOVE))
            {
                CheckMenuActive(move);
            }
            else if (!UIActive)
            {
                CheckPlayerMovement(move, actualControllerCurrPos);
            }
        }
    }

    /********************Menu Control***************************/
    private void CheckHoverOverMenuItems() {
        //Control menu items
        if (UIActive)
        {
            menuUIScript.clearSelection();
            LayerMask UIMask = LayerMask.GetMask("UI");
            GameObject mousePointedAt = HelperLibrary.WorldToScreenRaycast(moveCursor.position, camera, 1000, UIMask);
            if (isControllable && menuUIScript.playerMenuTransform.gameObject.activeSelf && mousePointedAt != null)
            {
                menuUIScript.HighlightItem(mousePointedAt);
            }
        }
    }

    private void CheckMenuSelected(GameObject menuItem)
    {
        if (menuItem.CompareTag("UI"))
        {
            menuUIScript.SelectItem(menuItem);
        }
    }

    private void CheckMenuActive(WinMoveController move)
    {
        Debug.Log("Checking menu");
        if (HelperLibrary.isTopRight(moveCursor.position, camera))
        {
            menuUIScript.ToggleMenu();
            UIActive = menuUIScript.isActive;
        }
    }

    /********************Tools***********************************/

    private void CheckToolClick(GameObject mousePointedAt)
    {
        // Check if this user is eligible to use this tool
        if (mousePointedAt.CompareTag("ResizeTool")
            && GameLogic.playerWhoIsUsingResizeTool == GameLogic.InvalidPlayerId)
        {
            // This player uses this tool.
            ToolInterface tool = mousePointedAt.transform.GetComponent<ToolInterface>();
            // Game logic stores unique ids for the player and the tool.
            GameLogic.TagResizePlayer(PhotonNetwork.player.ID, mousePointedAt.GetInstanceID());
            tool.Use(transform);
        }
        else if (mousePointedAt.CompareTag("SpringTool")
                 && GameLogic.playerWhoIsUsingSpringTool == GameLogic.InvalidPlayerId)
        {
            GameLogic.TagSpringPlayer(PhotonNetwork.player.ID, mousePointedAt.GetInstanceID());
            // This player uses this tool.
            ToolInterface tool = mousePointedAt.transform.GetComponent<ToolInterface>();
            tool.Use(transform);
        }
        else if (mousePointedAt.CompareTag("MagnetTool")
                 && GameLogic.playerWhoIsUsingMagnetTool == GameLogic.InvalidPlayerId)
        {
            GameLogic.TagMagnetPlayer(PhotonNetwork.player.ID, mousePointedAt.GetInstanceID());
            // This player uses this tool.
            ToolInterface tool = mousePointedAt.transform.GetComponent<ToolInterface>();
            tool.Use(transform);
        }
    }

    private void CheckPlayerClick(GameObject mousePointedAt)
    {
        if (mousePointedAt.CompareTag("Player"))
        {
            // if this player is not "controlling the resizing tool", 
            // the player can't tag anyone, so don't do anything on collision
            Debug.Log(PhotonNetwork.player.ID + " is resizing.");
            if (PhotonNetwork.player.ID == GameLogic.playerWhoIsUsingResizeTool
                && toolMap.ContainsKey(GameLogic.resizeTool))
            {
                // Get the root photon view of the player.
                PhotonView rootView =
                    mousePointedAt.transform.root.GetComponent<PhotonView>();
                // Set the resizing target.
                GameLogic.TagResizeTarget(rootView.owner.ID);
                ToolInterface tool = toolMap[GameLogic.resizeTool].GetComponent<ResizeTool>();
                tool.SetTarget(mousePointedAt.transform);

                //                Debug.Log("Set resize target to " + rootView.owner.ID);
            }
            else if (PhotonNetwork.player.ID == GameLogic.playerWhoIsUsingSpringTool)
            {
                // Get the root photon view of the player.
                PhotonView rootView =
                    mousePointedAt.transform.root.GetComponent<PhotonView>();
                // Set the resizing target.
                GameLogic.TagSpringTarget(rootView.owner.ID);
            }
            else if (PhotonNetwork.player.ID == GameLogic.playerWhoIsUsingMagnetTool)
            {
                // Get the root photon view of the player.
                PhotonView rootView =
                    mousePointedAt.transform.root.GetComponent<PhotonView>();
                // Set the resizing target.
                GameLogic.TagMagnetTarget(rootView.owner.ID);
            }
        }
    }

    private void CheckToolPerform()
    {
        if (PhotonNetwork.player.ID == GameLogic.playerWhoIsUsingResizeTool
            && toolMap.ContainsKey(GameLogic.resizeTool))
        {
            // Get the tool and start resize its target.
            ToolInterface tool = toolMap[GameLogic.resizeTool].GetComponent<ResizeTool>();

            tool.Perform(Input.mousePosition.y);

        }
    }

    /**********************Movement Control********************/

    private void CheckPlayerMovement(WinMoveController move, Vector3 actualControllerCurrPos)
    {
        //calculate delta position of move controller
        if (move.btnOnPress(MoveButton.BTN_MOVE))
        {
            setInitialPos(actualControllerCurrPos);
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

    public void setInitialPos(Vector3 pos)
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
            camera.transform.Rotate(-deltaPos.y, 0, 0);
        }
        else
        {
            Quaternion newAngle = Quaternion.LookRotation(new Vector3(deltaPos.x, 0, 0));
            playerRigidbody.MoveRotation(Quaternion.Lerp(transform.rotation, 
                    transform.rotation * newAngle, Time.deltaTime));
            transform.rotation = Quaternion.Lerp(transform.rotation,
            transform.rotation * newAngle, Time.deltaTime);
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
