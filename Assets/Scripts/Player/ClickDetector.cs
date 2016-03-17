using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/**
 * This class handles all the game logic that is gone through clicks.
 */ 
[RequireComponent(typeof(MenuUI))]
public class ClickDetector : MonoBehaviour
{
    MenuUI menu;
    public bool isControllable = false;
    Dictionary<int, GameObject> toolMap;
    int playerMask;
    int toolMask;
    int menuUIMask;
    int combinedMask;
    int playerID = GameLogic.InvalidPlayerId;
    float startHeight = 0.0f;
    float endHeight = 0.0f;
    // The max is about 36, min is 0.2 and player is 10, which is roughly 0.273
    // because 0.2 + 0.28*(36-0.2) ~= 10
    float defaultResize = 0.273f;

    void Awake()
    {
        menu = GetComponent<MenuUI>();
        toolMap = HelperLibrary.GetAllToolsInScene();
        playerMask = LayerMask.GetMask("Player");
        toolMask = LayerMask.GetMask("Tool");
        menuUIMask = LayerMask.GetMask("UI");
        combinedMask = playerMask | toolMask | menuUIMask;
    }

    void Update()
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

        // If left button of the mouse is held down,
        // and if the ray of the mouse touches another player,
        // start controlling the size of the other player
        // and change the camera view of the other player accordingly.
        // TODO(sainan): refactor the turn and move logic using mouse click
        // for move, and arrow to turn, and look up or down.
        if (isControllable && Input.GetMouseButtonUp(0))
        {
//            Debug.Log("Button up");
            RayCastReturnValue mousePointedAt = HelperLibrary.RaycastObject(Input.mousePosition, combinedMask);
            GameObject hitGameObj = mousePointedAt.hitObject;
            if (hitGameObj != null && hitGameObj != this.gameObject)
            {
//                Debug.Log("Hit a desired object" + hitGameObj.name + " with layer mask " + hitGameObj.layer);
                // Check if it is clicking a tool.
                CheckToolClick(hitGameObj);
                // Check if it is clicking a player. 
                CheckPlayerClick(hitGameObj);
                // Check if it is clicking a menu item.
                CheckMenuClick(hitGameObj);

                // Reset the stored mouse interaction for drag.
                if (startHeight > 0.0001f || endHeight > 0.0001f)
                {
                    startHeight = 0.0f;
                    endHeight = 0.0f;   
                }
            }
        }

        if (isControllable && Input.GetMouseButtonDown(0))
        {
            Debug.Log("Button down");
            // When long press, we only want to hit players.
            RayCastReturnValue mousePointedAt = HelperLibrary.RaycastObject(Input.mousePosition, playerMask);
            GameObject hitGameObj = mousePointedAt.hitObject;
            if (hitGameObj != null && hitGameObj != this.gameObject)
            {
                Debug.Log("hit " + hitGameObj.name);
                CheckToolPerform(hitGameObj);
            }
        }

        // Drag.
        if (isControllable && Input.GetMouseButton(0))
        {
            UpdateToolPerform();
        }
    }

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
        }
        else if (hitGameObj.CompareTag("SpringTool")
                 && GameLogic.playerWhoIsUsingSpringTool == GameLogic.InvalidPlayerId)
        {
            GameLogic.TagSpringPlayer(playerID, hitGameObj.GetInstanceID());
            // This player uses this tool.
            ToolInterface tool = hitGameObj.transform.GetComponent<ToolInterface>();
            tool.Use(transform);
        }
        else if (hitGameObj.CompareTag("MagnetTool")
                 && GameLogic.playerWhoIsUsingMagnetTool == GameLogic.InvalidPlayerId)
        {
            GameLogic.TagMagnetPlayer(playerID, hitGameObj.GetInstanceID());
            // This player uses this tool.
            ToolInterface tool = hitGameObj.transform.GetComponent<ToolInterface>();
            tool.Use(transform);
        }
    }

    private void CheckPlayerClick(GameObject hitGameObj)
    {
        if (hitGameObj.CompareTag("Player"))
        {
            if (playerID == GameLogic.playerWhoIsUsingResizeTool
                && toolMap.ContainsKey(GameLogic.resizeTool))
            {
                // if this player is not "controlling the resizing tool", 
                // the player can't tag anyone, so don't do anything on collision
                Debug.Log(PhotonNetwork.player.ID + " is being resized.");
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

    private void CheckMenuClick(GameObject hitGameObj)
    {
        if (hitGameObj.CompareTag("UI"))
        {
            menu.SelectItem(hitGameObj);
        }
    }

    private void CheckToolPerform(GameObject hitGameObj)
    {
        // Check if the player is operating on some other player.
        if (playerID == GameLogic.playerWhoIsUsingResizeTool
            && toolMap.ContainsKey(GameLogic.resizeTool))
        {
            if (GameLogic.playerWhoIsBeingResized == GameLogic.InvalidPlayerId)
            {
                // The short click was not detected, so we set the resize target before we resize.
                SetResizeTarget(hitGameObj);
            }

            startHeight = Input.mousePosition.y;
        }
    }

    private void UpdateToolPerform()
    {
        if (playerID != GameLogic.InvalidPlayerId && playerID == GameLogic.playerWhoIsUsingResizeTool
            && GameLogic.playerWhoIsBeingResized != GameLogic.InvalidPlayerId && toolMap.ContainsKey(GameLogic.resizeTool)
            && startHeight != 0.0f)
        {
            endHeight = Input.mousePosition.y;
            // Scale this number down to [-1, 1].
            float scrollSpeed = (endHeight - startHeight) / Screen.height;
            Debug.Log("Scrolling at speed" + scrollSpeed);
            float scaleFactor = defaultResize + scrollSpeed;
            if (scaleFactor < 0)
            {
                scaleFactor = 0.0f;
            }
            if (scaleFactor > 1)
            {
                scaleFactor = 1.0f;
            }

            // Get the tool and start resize its target.
            ToolInterface tool = toolMap[GameLogic.resizeTool].GetComponent<ResizeTool>();

            // TryPerform takes in a number between 0 and 1. 0 being the smallest scaling factor available on screen.
            tool.TryPerform(scaleFactor);
        }

    }
}
