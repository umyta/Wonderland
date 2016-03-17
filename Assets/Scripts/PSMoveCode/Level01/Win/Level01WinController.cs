using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using MoveServerNS;

//Sony move server
[RequireComponent(typeof(MenuUI))]
[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(Rigidbody))]
public class Level01WinController : MonoBehaviour, MotionController
{
    public Transform playerCameraTransform;
    public Transform moveCursor;
    public Vector3 moveCursorInitPos;

    // Player Camera
    private Camera playerCamera;
    //Windows Server
    private WinMoveServer moveServer;

    [Range(1, 6)]
    public int controller;
    //public GameObject controlBall;
    public PlayerState state;
    public PlayerState toolState = PlayerState.Idle;
    public bool isControllable = false;

    //UI
    private Vector3 actualControllerPrevPos;
    private Vector3 delta;
    MenuUI menuUIScript;
    private bool UIActive;
    private bool setUIInitial;
    private float SCALE = 15.0f;

    //Tools
    Dictionary<int, GameObject> toolMap;
    int playerMask;
    int toolMask;
    int menuUIMask;
    int combinedMask;
    int playerID = GameLogic.InvalidPlayerId;
    float initialY;

    //Clues
    ClueState clueState = ClueState.None;

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
        playerCamera = GetComponent<Camera>();
        toolMap = HelperLibrary.GetAllToolsInScene();
        moveCursorInitPos = moveCursor.transform.localPosition;
    }

    // Update is called once per frame
    public void FixedUpdate()
    {
        ErrorCheck();

        if (moveServer == null || toolMap == null)
        {
            Debug.LogWarning("Please set up move server or toolMap first");
            return;
        }
        WinMoveController move = moveServer.getController(controller - 1);

        if (playerCamera == null)
        {
            playerCamera = transform.GetComponentInChildren<Camera>();
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

                delta += transform.forward * delta.z;
                delta += transform.up * delta.y;
                delta += transform.right * delta.x;
                delta.z = -delta.z;

                moveCursor.position += delta * SCALE;
            }

            /* Trigger function: either exit tool or reset move cursor */
            if (move.triggerValue > 0)
            {
                if (toolState == PlayerState.Tool)
                {
                    Debug.Log("End using camera.");
                    CheckKeyExit();
                }
                else
                {
                    moveCursor.localPosition = moveCursorInitPos;
                    moveCursor.transform.forward = playerCamera.transform.forward;
                }
            }

            /* Exit menu */
            if (HelperLibrary.isTopRight(moveCursor.position, playerCamera) && move.btnOnRelease(MoveButton.BTN_MOVE))
            {
                CheckMenuActive(move);
                return;
            }

            /* Hover */
            CheckHoverOverMenuItems();

            RayCastReturnValue mousePointedAt = HelperLibrary.WorldToScreenRaycast(moveCursor.position, playerCamera, 1000);
            GameObject hitObject = mousePointedAt.hitObject;            

            if (hitObject != null && hitObject != this.gameObject && move.btnOnRelease(MoveButton.BTN_MOVE))
            {
                Debug.Log("Raycast hit " + mousePointedAt.hitObject.name);
                CheckMenuSelected(hitObject);
                CheckToolClick(hitObject);
            }

            /* Tool operations */
            if (toolState == PlayerState.Tool && move.btnOnPress(MoveButton.BTN_MOVE))
                initialY = moveCursor.position.y;
            else if (toolState == PlayerState.Tool && move.btnPressed(MoveButton.BTN_MOVE))
            {
                CheckToolPerform(moveCursor.position.y / initialY);
                Debug.Log("Scaling at " + moveCursor.position.y / initialY);
            }


            /* Player Movement */
            if (!UIActive)
            {
                CheckPlayerMovement(move, actualControllerCurrPos);
            }
        }
    }

    /********************Menu Control***************************/
    private void CheckHoverOverMenuItems()
    {
        //Control menu items
        if (UIActive)
        {
            menuUIScript.clearSelection();
            LayerMask UIMask = LayerMask.GetMask("UI");
            RayCastReturnValue mousePointedAt = HelperLibrary.WorldToScreenRaycast(moveCursor.position, playerCamera, 1000, UIMask);
            GameObject hitObject = mousePointedAt.hitObject;
            if (isControllable && menuUIScript.playerMenuTransform.gameObject.activeSelf && hitObject != null)
            {
                menuUIScript.HighlightItem(hitObject);
            }
        }
    }

    private void ErrorCheck()
    {
        // Make sure the toolMap is set.
        if (toolMap == null)
        {
            Debug.Log("toolMap is missing.");

            return;
        }

        // Make sure that player ID is valid.
        if (playerID == GameLogic.InvalidPlayerId)
        {
            // Try to setup the player ID.
            playerID = PhotonNetwork.player.ID;
        }

        if (playerID == GameLogic.InvalidPlayerId)
        {
            // If the setup fails, the playerID may still be invalid.
            return;
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
       menuUIScript.ToggleMenu();
       UIActive = menuUIScript.isActive;
    }

    /********************Tools***********************************/

    private void CheckToolClick(GameObject hitGameObj)
    {
        // Check if this user is eligible to use this tool
        if (hitGameObj.CompareTag("ResizeTool")
            && GameLogic.playerWhoIsUsingResizeTool == GameLogic.InvalidPlayerId)
        {
            // This player uses this tool.
            ToolInterface tool = hitGameObj.transform.GetComponent<ToolInterface>();
            // Game logic stores unique ids for the player and the tool.
            GameLogic.TagResizePlayer(playerID, hitGameObj.GetInstanceID());
            tool.Use(transform);
            toolState = PlayerState.Tool;
            if (ClueState.Clue1 > clueState)
                clueState = ClueState.Clue1;
        }
        else if (hitGameObj.CompareTag("SpringTool")
                 && GameLogic.playerWhoIsUsingSpringTool == GameLogic.InvalidPlayerId)
        {
            GameLogic.TagSpringPlayer(playerID, hitGameObj.GetInstanceID());
            // This player uses this tool.
            ToolInterface tool = hitGameObj.transform.GetComponent<ToolInterface>();
            tool.Use(transform);
            toolState = PlayerState.Tool;
        }
        else if (hitGameObj.CompareTag("MagnetTool")
                 && GameLogic.playerWhoIsUsingMagnetTool == GameLogic.InvalidPlayerId)
        {
            GameLogic.TagMagnetPlayer(playerID, hitGameObj.GetInstanceID());
            // This player uses this tool.
            ToolInterface tool = hitGameObj.transform.GetComponent<ToolInterface>();
            tool.Use(transform);
            toolState = PlayerState.Tool;
        }
    }

    private void CheckPlayerClick(GameObject hitGameObj)
    {
        if (hitGameObj.CompareTag("Player"))
        {
            // if this player is not "controlling the resizing tool", 
            // the player can't tag anyone, so don't do anything on collision
            Debug.Log(PhotonNetwork.player.ID + " is being resized.");
            if (playerID == GameLogic.playerWhoIsUsingResizeTool
                && toolMap.ContainsKey(GameLogic.resizeTool))
            {
                SetResizeTarget(hitGameObj);
                //                Debug.Log("Set resize target to " + rootView.owner.ID);
            }
            else if (playerID == GameLogic.playerWhoIsUsingSpringTool)
            {
                // Get the root photon view of the player.
                PhotonView rootView =
                    hitGameObj.transform.root.GetComponent<PhotonView>();
                // Set the resizing target.
                GameLogic.TagSpringTarget(rootView.owner.ID);
            }
            else if (playerID == GameLogic.playerWhoIsUsingMagnetTool)
            {
                // Get the root photon view of the player.
                PhotonView rootView =
                    hitGameObj.transform.root.GetComponent<PhotonView>();
                // Set the resizing target.
                GameLogic.TagMagnetTarget(rootView.owner.ID);
            }
        }
    }

    private void SetResizeTarget(GameObject hitGameObj)
    {
        // Get the root photon view of the player.
        PhotonView rootView =
            hitGameObj.transform.root.GetComponent<PhotonView>();
        // Set the resizing target.
        GameLogic.TagResizeTarget(rootView.owner.ID);
        ToolInterface tool = toolMap[GameLogic.resizeTool].GetComponent<ResizeTool>();
        tool.SetTarget(hitGameObj.transform);
    }

    private void CheckToolPerform(float scale)
    {
        if (PhotonNetwork.player.ID == GameLogic.playerWhoIsUsingResizeTool
            && toolMap.ContainsKey(GameLogic.resizeTool))
        {
            // Get the tool and start resize its target.
            ToolInterface tool = toolMap[GameLogic.resizeTool].GetComponent<ResizeTool>();

            tool.TryPerform(scale);
        }
    }

    private void CheckKeyExit()
    {
        // Does this current player controls a resize tool.
        if (GameLogic.playerWhoIsUsingResizeTool != PhotonNetwork.player.ID
            && toolMap.ContainsKey(GameLogic.resizeTool))
        {
            toolMap[GameLogic.resizeTool].GetComponent<ResizeTool>().Done();
            // Reset game logic for resizeing.
            GameLogic.TagResizePlayer(GameLogic.InvalidPlayerId, GameLogic.InvalidToolId);
        }
        state = PlayerState.Idle;
        // TODO(sainan): setup the exit logic for other tools.
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
            playerCamera.transform.Rotate(-deltaPos.y, 0, 0);
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
