﻿using UnityEngine;
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
    Dictionary<int, GameObject> toolMap = new Dictionary<int, GameObject>();

    void Awake()
    {
        menu = GetComponent<MenuUI>();
        // TODO(sainan): consider tag all tools as Tool instead of individual tags.
        // Considering that we may need to check type of classes to differentiate game logic
        // in clickDetector.
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

    void Update()
    {
        // If left button of the mouse is held down,
        // and if the ray of the mouse touches another player,
        // start controlling the size of the other player
        // and change the camera view of the other player accordingly.
        // TODO(sainan): refactor the turn and move logic using mouse click
        // for move, and arrow to turn, and look up or down.
        if (isControllable && Input.GetMouseButtonUp(0))
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

        if (isControllable && Input.GetMouseButtonDown(0))
        {
            RayCastReturnValue mousePointedAt = HelperLibrary.RaycastObject(Input.mousePosition);
            GameObject hitGameObj = mousePointedAt.hitObject;
            if (hitGameObj != null && hitGameObj != this.gameObject)
            {
                CheckToolPerform();
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

    private void CheckMenuClick(GameObject mousePointedAt)
    {
        if (mousePointedAt.CompareTag("UI"))
        {
            menu.SelectItem(mousePointedAt);
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
}
