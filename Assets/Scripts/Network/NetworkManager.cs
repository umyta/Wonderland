using UnityEngine;
using System.Collections;

public class NetworkManager : MonoBehaviour
{
    private const string VERSION = "v0.0.1";
    private const int GROUP = 0;
    public string playerPrefabName = "NetworkPlayer";
    public string roomName = "Test";
    public Transform spawnPopint;

    void Start()
    {
        // Enable photon network.
        if (!PhotonNetwork.autoJoinLobby)
        {
            PhotonNetwork.autoJoinLobby = true;
        }
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

    void OnJoinedLobby()
    {
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
        PhotonNetwork.Instantiate(
            playerPrefabName,
            spawnPopint.position,
            spawnPopint.rotation, 
            GROUP);
    }
}
