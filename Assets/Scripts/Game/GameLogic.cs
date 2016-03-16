using UnityEngine;
using System.Collections;


[RequireComponent((typeof(PhotonView)))]
public class GameLogic : MonoBehaviour
{
    public const int InvalidPlayerId = -1;
    public const int InvalidToolId = -1;
    // Only one player can use one type of tool at one time.
    public static int playerWhoIsUsingResizeTool = InvalidPlayerId;
    public static int playerWhoIsUsingSpringTool = InvalidPlayerId;
    public static int playerWhoIsUsingMagnetTool = InvalidPlayerId;

    // Keeps track of what tool is in use.
    public static int resizeTool = InvalidToolId;
    public static int springTool = InvalidToolId;
    public static int magnetTool = InvalidToolId;

    public static int playerWhoIsBeingResized = InvalidPlayerId;
    public static int playerWhoIsBeingSpringed = InvalidPlayerId;
    public static int playerWhoIsBeingMagnetized = InvalidPlayerId;

    private static PhotonView ScenePhotonView;

    void Start()
    {
        ScenePhotonView = this.GetComponent<PhotonView>();
    }

    #region Static functions

    public static void TagResizePlayer(int playerID, int toolID)
    {
        Debug.Log("TagResizePlayer: " + playerID + " with tool " + toolID);
        ScenePhotonView.RPC("TaggedResizePlayer", PhotonTargets.All, playerID, toolID);
    }

    public static void TagResizeTarget(int playerID)
    {
        Debug.Log("TagResizeTarget: " + playerID);
        ScenePhotonView.RPC("TaggedResizeTarget", PhotonTargets.All, playerID);
    }

    public static void TagSpringPlayer(int playerID, int toolID)
    {
        Debug.Log("TagSpringPlayer: " + playerID + " with tool " + toolID);
        ScenePhotonView.RPC("TaggedSpringPlayer", PhotonTargets.All, playerID, toolID);
    }

    public static void TagSpringTarget(int playerID)
    {
        Debug.Log("TagSpringTarget: " + playerID);
        ScenePhotonView.RPC("TaggedSpringTarget", PhotonTargets.All, playerID);
    }

    public static void TagMagnetPlayer(int playerID, int toolID)
    {
        Debug.Log("TagMagnetPlayer: " + playerID + " with tool " + toolID);
        ScenePhotonView.RPC("TaggedMagnetPlayer", PhotonTargets.All, playerID, toolID);
    }

    public static void TagMagnetTarget(int playerID)
    {
        Debug.Log("TagMagnetTarget: " + playerID);
        ScenePhotonView.RPC("TaggedMagnetTarget", PhotonTargets.All, playerID);
    }

    #endregion

    // If a player is disconnected, reset the player who is using the resize
    // tool and the player that is being resized.
    void OnPhotonPlayerDisconnected(PhotonPlayer player)
    {
        Debug.Log("OnPhotonPlayerDisconnected: " + player);

        if (PhotonNetwork.isMasterClient)
        {
            if (player.ID == playerWhoIsUsingResizeTool)
            {
                // TODO(sainan): make sure the player puts down the tool when they are leaving.
                // if the player who left was "it", reset the player who is using the resize tool.
                TagResizePlayer(InvalidPlayerId, InvalidToolId);
                TagResizeTarget(InvalidPlayerId);
            }
        }
    }

    #region RPC functions

    // RunRPC function can be called remotely. The GameObject needs to have photonview.

    [PunRPC]
    void TaggedResizePlayer(int playerID, int toolID)
    {
        playerWhoIsUsingResizeTool = playerID;
        resizeTool = toolID;
        Debug.Log("TaggedResizePlayer: " + playerID);
    }

    [PunRPC]
    void TaggedSpringPlayer(int playerID, int toolID)
    {
        playerWhoIsUsingSpringTool = playerID;
        springTool = toolID;
        Debug.Log("TaggedSpringPlayer: " + playerID);
    }

    [PunRPC]
    void TaggedMagenetPlayer(int playerID, int toolID)
    {
        playerWhoIsUsingMagnetTool = playerID;
        springTool = toolID;
        Debug.Log("TaggedMagnetPlayer: " + playerID);
    }
    // RunRPC function can be called remotely. The GameObject needs to have photonview.
    [PunRPC]
    void TaggedResizeTarget(int playerID)
    {
        playerWhoIsBeingResized = playerID;
        Debug.Log("TaggedPlayerBeingResized: " + playerID);
    }

    [PunRPC]
    void TaggedSpringTarget(int playerID)
    {
        playerWhoIsBeingSpringed = playerID;
        Debug.Log("TaggedPlayerBeingSpringed: " + playerID);
    }

    [PunRPC]
    void TaggedMagenetTarget(int playerID)
    {
        playerWhoIsBeingMagnetized = playerID;
        Debug.Log("TaggedPlayerBeingMagnetized: " + playerID);
    }

    #endregion
}
