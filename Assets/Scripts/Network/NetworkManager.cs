using UnityEngine;
using System.Collections;
using Photon;

public class NetworkManager : Photon.PunBehaviour
{
    private const string VERSION = "v0.0.1";
    private const int GROUP = 0;
    public string playerPrefabName = "NetworkPlayer";
    public string roomName = "Test";
    public Transform spawnPopint;

    void Start()
    {
        // Enable photon network. This should be set in Resources/PhotonServerSettings.
        // In case it is not set:
        if (!PhotonNetwork.autoJoinLobby)
        {
            PhotonNetwork.autoJoinLobby = true;
        }
        // Use PhotonServerSettings.
        PhotonNetwork.ConnectUsingSettings(VERSION);
	
    }

    void OnGUI()
    {
        // Prints current status
        if (PhotonNetwork.autoJoinLobby)
        {
            // Debug.Log ("Connecting");
            GUILayout.Label(PhotonNetwork.connectionStateDetailed.ToString());
        }
        else
        {
            // Debug.Log ("Offline");
            GUILayout.Label("autoJoinLoby is disabled.");
        }
    }

    public override void OnJoinedLobby()
    {
        // For debugging. This will show photon logs in console.
        // We are using default log level right now.
        // PhotonNetwork.logLevel = PhotonLogLevel.Full;

        RoomOptions roomOptions = new RoomOptions()
        { 
            isVisible = false, maxPlayers = 4
        };
        PhotonNetwork.JoinOrCreateRoom(
            roomName, roomOptions, TypedLobby.Default);
    }

    void OnJoinedRoom()
    {
        // Instantiate everything that every user should see here.
        GameObject player = PhotonNetwork.Instantiate(
                                playerPrefabName,
                                spawnPopint.position,
                                spawnPopint.rotation, 
                                GROUP);
        // Enable player controllers.
        MouseKeyboardCharacterControl mkController = player.GetComponent<MouseKeyboardCharacterControl>();
        // Instead of disabling componenets, we use a bool here to differentiate serialization.
        mkController.isControllable = true;
        // Set camera to follow this player when it's my turn.
        GameObject.FindObjectOfType<CameraFollow>().SetTarget(player.transform);
    }

    void OnPhotonRandomJoinFailed()
    {
        Debug.Log("Can't join random room!");
    }
}
