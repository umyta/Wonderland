using UnityEngine;
using System.Collections;

[RequireComponent(typeof(ResizeTool))]
public class NetworkTool : MonoBehaviour
{
    private Vector3 correctPlayerPos = Vector3.zero;
    private Quaternion correctPlayerRot = Quaternion.identity;
    private PhotonView photonView;

    void Start()
    {
        photonView = GetComponent<PhotonView>();
    }
    // Update is called once per frame
    void Update()
    {
        // Instead of simply update transform, we want the transforms to be smooth.
        if (!photonView.isMine)
        {
            transform.position = Vector3.Lerp(transform.position, this.correctPlayerPos, Time.deltaTime * 5);
            transform.rotation = Quaternion.Lerp(transform.rotation, this.correctPlayerRot, Time.deltaTime * 5);
        }
    }

    void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.isWriting)
        {
            // We own this player: send the others our data
            stream.SendNext(transform.position);
            stream.SendNext(transform.rotation);
            // Send animations across the network.
//            stream.SendNext(resizeTool.GetStatus().flag);
        }
        else
        {
            // Network player, receive data
//            this.correctPlayerPos = (Vector3)stream.ReceiveNext();
//            this.correctPlayerRot = (Quaternion)stream.ReceiveNext();
            // Interpret animations from the network.
//            resizeTool.EnablePhysics((bool)stream.ReceiveNext());
        }
    }
}
