using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;

public class GameLogicDisplay : MonoBehaviour
{

    private const string GAME_LOGIC_STATUS = "GameLogicStatus";
    private Text gameStatus;
    Dictionary<int, GameObject> toolMap;
    // Use this for initialization
    void Start()
    {
        gameStatus = GameObject.Find(GAME_LOGIC_STATUS).GetComponent<Text>();
        toolMap = HelperLibrary.GetAllToolsInScene();
    }
	
    // Update is called once per frame
    void OnGUI()
    {
        if (gameStatus == null)
        {
            Debug.LogWarning("Please place a Text named " + GAME_LOGIC_STATUS + " under Canvas UI in the scene.");
            return;
        }

        int myPlayerID = PhotonNetwork.player.ID;

        if (myPlayerID == GameLogic.InvalidPlayerId)
        {
            return;
        }

        if (myPlayerID == GameLogic.playerWhoIsUsingResizeTool)
        {
            if (!toolMap.ContainsKey(GameLogic.resizeTool))
            {
                Debug.LogWarning("player " + myPlayerID + " is using resizing, but toolMap does not contain " + GameLogic.resizeTool);
                return;
            }

            ResizeTool tool = toolMap[GameLogic.resizeTool].GetComponent<ResizeTool>();
            gameStatus.text = 
                "Player is Scaling " + GameLogic.playerWhoIsBeingResized +
            " by " + tool.GetStatus().factor + "%\n";
        }
        else if (myPlayerID == GameLogic.playerWhoIsUsingSpringTool)
        {
            gameStatus.text = "Player is picking up : " + GameLogic.playerWhoIsBeingSpringed + "\n";
                
        }
        else if (myPlayerID == GameLogic.playerWhoIsUsingMagnetTool)
        {
            gameStatus.text = "Player is pulling: " + GameLogic.playerWhoIsBeingMagnetized + "\n";
        }
        else
        {
            gameStatus.text = "Look for a clue!";
        }
    }
}
