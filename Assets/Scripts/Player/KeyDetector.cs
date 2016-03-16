using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/**
 * This class handles all the game logic through key presses
 */ 
[RequireComponent(typeof(PhotonView))]
[RequireComponent(typeof(MenuUI))]
public class KeyDetector : MonoBehaviour
{
    public bool isControllable = false;
    Dictionary<int, GameObject> toolMap = new Dictionary<int, GameObject>();
    PhotonView photonView;
    MenuUI menu;

    void Awake()
    {
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

        photonView = transform.GetComponent<PhotonView>();
        menu = transform.GetComponent<MenuUI>();
    }
    // Update is called once per frame
    void Update()
    {
        if (isControllable && Input.GetKeyDown(KeyCode.Home))
        {
            CheckKeyExit();
        }

        if (isControllable && Input.GetKeyDown(KeyCode.Space))
        {
            Debug.Log("Space key is pressed");
            CheckMenu();
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
        // TODO(sainan): setup the exit logic for other tools.
    }

    private void CheckMenu()
    {
        if (photonView.isMine)
        {
            menu.ToggleMenu();
        }
        else
        {
            Debug.Log("Control is not mine");
        }
    }
}
