using UnityEngine;
using System.Collections;

public class NetworkPlayer : Photon.MonoBehaviour
{
    private Vector3 correctPlayerPos = Vector3.zero;
    private Quaternion correctPlayerRot = Quaternion.identity;
    private Vector3 correctPlayerScale = Vector3.zero;

    // Update is called once per frame
    void Update()
    {
        // Instead of simply update transform, we want the transforms to be smooth.
        if (!photonView.isMine)
        {
            transform.position = Vector3.Lerp(transform.position, this.correctPlayerPos, Time.deltaTime * 5);
            transform.rotation = Quaternion.Lerp(transform.rotation, this.correctPlayerRot, Time.deltaTime * 5);
            transform.localScale = Vector3.Lerp(transform.localScale, this.correctPlayerScale, Time.deltaTime * 5);
        }
    }

    void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.isWriting)
        {
            // We own this player: send the others our data
            stream.SendNext(transform.position);
            stream.SendNext(transform.rotation);
            stream.SendNext(transform.localScale);
            // Send animations across the network.
            MouseKeyboardCharacterControl mkControl = GetComponent<MouseKeyboardCharacterControl>();
            stream.SendNext((int)mkControl.state);
        }
        else
        {
            // Network player, receive data
            this.correctPlayerPos = (Vector3)stream.ReceiveNext();
            this.correctPlayerRot = (Quaternion)stream.ReceiveNext();
            this.correctPlayerScale = (Vector3)stream.ReceiveNext();
            // Interpret animations from the network.
            MouseKeyboardCharacterControl myC = GetComponent<MouseKeyboardCharacterControl>();
            myC.SetAnimationState((PlayerState)stream.ReceiveNext());
        }
    }
}

