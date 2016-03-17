using UnityEngine;
using System.Collections;

[RequireComponent(typeof(ResizeTool))]
public class NetworkTool : MonoBehaviour
{
    private Vector3 correctPose = Vector3.zero;
    private Quaternion correctRot = Quaternion.identity;

    void Start()
    {

    }
    // Update is called once per frame
    void Update()
    {
        // Instead of simply update transform, we want the transforms to be smooth.
        if (!transform.GetComponent<PhotonView>().isMine)
        {
            transform.position = Vector3.Lerp(transform.position, this.correctPose, Time.deltaTime * 5);
            transform.rotation = Quaternion.Lerp(transform.rotation, this.correctRot, Time.deltaTime * 5);
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
            this.correctPose = (Vector3)stream.ReceiveNext();
            this.correctRot = (Quaternion)stream.ReceiveNext();
            // Interpret animations from the network.
//            resizeTool.EnablePhysics((bool)stream.ReceiveNext());
        }
    }
}
