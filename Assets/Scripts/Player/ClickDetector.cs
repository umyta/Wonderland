using UnityEngine;
using System.Collections;

/**
 * This class handles all the game logic that is gone through clicks.
 */ 
[RequireComponent(typeof(MenuUI))]
public class ClickDetector : MonoBehaviour
{
    MenuUI menu;

    void Awake()
    {
        menu = GetComponent<MenuUI>();
    }

    void Update()
    {
        // If left button of the mouse is held down,
        // and if the ray of the mouse touches another player,
        // start controlling the size of the other player
        // and change the camera view of the other player accordingly.
        // TODO(sainan): refactor the turn and move logic using mouse click
        // for move, and arrow to turn, and look up or down.
        if (Input.GetMouseButton(0))
        {
            RayCastReturnValue mousePointedAt = HelperLibrary.RaycastObject(Input.mousePosition);
            GameObject hitGameObj = mousePointedAt.hitObject;
            if (hitGameObj != null && hitGameObj != this.gameObject)
            {
                // Check if it is clicking a tool.
                CheckToolClick(hitGameObj);
                // Check if it is clicking a player. 
                CheckPlayerClick(hitGameObj);
                // Check if it is clicking a menu item.
                CheckMenuClick(hitGameObj);
            }
        }
    }

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
            if (PhotonNetwork.player.ID == GameLogic.playerWhoIsUsingResizeTool)
            {
                // Get the root photon view of the player.
                PhotonView rootView = 
                    mousePointedAt.transform.root.GetComponent<PhotonView>();
                // Set the resizing target.
                GameLogic.TagResizeTarget(rootView.owner.ID);   
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

    private void CheckMenuClick(GameObject mousePointedAt)
    {
        if (mousePointedAt.CompareTag("UI"))
        {
            menu.SelectItem(mousePointedAt);
        }
    }
}
