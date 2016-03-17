using UnityEngine;
using System.Collections;
using Photon;
using MoveServerNS;

[RequireComponent(typeof(WinMoveServer))]
public class NetworkManager : Photon.PunBehaviour
{
    private const string VERSION = "v0.0.1";
    private const int GROUP = 0;
    public string playerPrefabName = "NetworkPlayer";
    public string roomName = "Test";
    public Transform spawnPopint;
    private WinMoveServer winMoveServer;

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
        winMoveServer = GetComponent<WinMoveServer>();
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

        // Enable mouse interaction movement.
        ClickDetector cDetector = player.GetComponent<ClickDetector>();
        KeyDetector kDetector = player.GetComponent<KeyDetector>();
        MouseOverDetector moDetector = player.GetComponent<MouseOverDetector>();
        PlayerStatusDisplay statusDisplay = player.GetComponent<PlayerStatusDisplay>();

        cDetector.isControllable = true;
        kDetector.isControllable = true;
        moDetector.isControllable = true;
        statusDisplay.isControllable = true;
        // Enable player camera.
        foreach (Transform child in player.transform)
        {
            if (child.name == "PlayerCamera")
            {
                child.gameObject.SetActive(true);
            }
        }
        // Detects the system and decide on what controller to enable.
        MouseKeyboardCharacterControl mkController = player.GetComponent<MouseKeyboardCharacterControl>();
        Level01WinController psmoveController = player.GetComponent<Level01WinController>();

        switch (HelperLibrary.GetOS())
        {
            case OperatingSystemForController.Mac:
                Debug.Log("Detected Mac system, no PSMove is setup here, please use mouse to control your character.");
                // Enable player controllers.
                mkController.isControllable = true;
                break;
            case OperatingSystemForController.Windows:
                Debug.Log("Detected Windows system:");
                if (HelperLibrary.HasMoveControllers(winMoveServer))
                {
                    Debug.Log("Found psmove controller! Please use your move to control the player.");
                    psmoveController.isControllable = true;
                }
                else
                {
                    Debug.Log("Did not find psmove controller! Please use your mouse to control the player.");
                    mkController.isControllable = true;
                }
                break;
            default:
                Debug.LogWarning("Unknown system detected!");
                break;
        }
    }

    void OnPhotonRandomJoinFailed()
    {
        Debug.Log("Can't join random room!");
    }
}
