using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class PlayerStatusDisplay : MonoBehaviour
{
    const string PLAYER_STATUS = "PlayerStatus";
    public bool isControllable = false;
    private Text playerStatus;
    // Use this for initialization
    void Start()
    {
        playerStatus = GameObject.Find(PLAYER_STATUS).GetComponent<Text>();
    }
	
    // Update is called once per frame
    void Update()
    {
        if (playerStatus == null)
        {
            Debug.LogWarning("Missing " + PLAYER_STATUS + " in Canvas UI");
            return;
        }

        if (!isControllable)
        {
            return;
        }
        // First clean the previous text.
        playerStatus.text = "";

        // Then update with the new text.
        int myPlayerID = PhotonNetwork.player.ID;
        if (myPlayerID == GameLogic.InvalidPlayerId)
        {
            // Player ID has not yet been set.
            return;
        }
        if (GameLogic.playerWhoIsBeingResized == myPlayerID)
        {
            playerStatus.text = "Player is Scaled to: " + transform.localScale + "\n";
        }
        if (GameLogic.playerWhoIsBeingSpringed == myPlayerID)
        {
            playerStatus.text += "Player is being picked up by: " + GameLogic.playerWhoIsUsingSpringTool + "\n";
        }
        if (GameLogic.playerWhoIsBeingMagnetized == myPlayerID)
        {
            playerStatus.text += "Player is being drawn to: " + GameLogic.playerWhoIsUsingMagnetTool + "\n";
        }
    }
}
