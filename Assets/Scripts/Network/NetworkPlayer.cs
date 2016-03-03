using UnityEngine;
using System.Collections;

public class NetworkPlayer : Photon.MonoBehaviour
{
	public GameObject myCamera;

	void Start ()
	{
		if (photonView.isMine) {
			// Turn on everything that may cause conflicts during network playing.
			myCamera.SetActive (true);	
		}
	}
	
	// Update is called once per frame
	void Update ()
	{
	
	}
}
