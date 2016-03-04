using UnityEngine;
using System.Collections;

public class NetworkPlayer : Photon.MonoBehaviour
{
    void Start()
    {
        if (photonView.isMine)
        {
            // Set camera to follow this player when it's my turn.
            GameObject.FindObjectOfType<CameraFollow>().SetTarget(transform);
        }
    }
	
    // Update is called once per frame
    void Update()
    {
	
    }
}
