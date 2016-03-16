using UnityEngine;
using System.Collections;

public class NetworkPlayer : Photon.MonoBehaviour
{
    private Vector3 correctPlayerPos;
    private Quaternion correctPlayerRot;

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
            MouseKeyboardCharacterControl mkController = GetComponent<MouseKeyboardCharacterControl>();
            stream.SendNext((int)mkController.state);
        }
        else
        {
            // Network player, receive data
            this.correctPlayerPos = (Vector3)stream.ReceiveNext();
            this.correctPlayerRot = (Quaternion)stream.ReceiveNext();
            // Interpret animations from the network.
            MouseKeyboardCharacterControl myC = GetComponent<MouseKeyboardCharacterControl>();
            myC.SetAnimationState((MouseKeyboardCharacterControl.PlayerState)stream.ReceiveNext());
        }
    }
}
