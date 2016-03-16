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

    public override void OnJoinedRoom()
    {
        // Instantiate everything that every user should see here.
        GameObject player = PhotonNetwork.Instantiate(
                                playerPrefabName,
                                spawnPopint.position,
                                spawnPopint.rotation, 
                                GROUP);
        
        // Instead of disabling componenets, we use a bool here to differentiate serialization.
        GameObject camObj = GameObject.Find("StandbyCamera");
        // Set to follow the player.
        CameraFollow follow = camObj.GetComponent<CameraFollow>();
        follow.SetTarget(player.transform);
        // Disable the main camera.
        camObj.SetActive(false);
        // Enable player controllers.
        MouseKeyboardCharacterControl mkController = player.GetComponent<MouseKeyboardCharacterControl>();

        ClickDetector cDetector = player.GetComponent<ClickDetector>();
        KeyDetector kDetector = player.GetComponent<KeyDetector>();
        MouseOverDetector moDetector = player.GetComponent<MouseOverDetector>();
        mkController.isControllable = true;
        cDetector.isControllable = true;
        kDetector.isControllable = true;
        moDetector.isControllable = true;
        // Enable player camera.
        foreach (Transform child in player.transform)
        {
            if (child.name == "PlayerCamera")
            {
                child.gameObject.SetActive(true);
            }
        }
    }

    void OnPhotonRandomJoinFailed()
    {
        Debug.Log("Can't join random room!");
    }
}
