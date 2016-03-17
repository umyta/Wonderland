using UnityEngine;
using System.Collections;

public class FollowPlayer : MonoBehaviour
{
	public float heightDamping = 2.0f;
	// The distance in the x-z plane to the target
	public float distance = 5.0f;
	// the height we want the camera to be above the target
	public float height = 5.0f;
	// How fast does this camera turn.
	public float rotationDamping = 0.1f;

	// Allow the resize tool to follow the player around while using.
	public void PlayerFollowTarget (Transform user, Transform target)
	{
		if (user == null) {
			return;
		}

		// Calculate the current rotation angles
		//      float wantedRotationAngle = user.eulerAngles.y;
		float wantedHeight = user.position.y + height;

		//        float currentRotationAngle = transform.eulerAngles.y;
		float currentHeight = transform.position.y;

		//        // Damp the rotation around the y-axis
		//        currentRotationAngle = Mathf.LerpAngle(currentRotationAngle, wantedRotationAngle, rotationDamping * Time.deltaTime);

		// Damp the height
		currentHeight = Mathf.Lerp (currentHeight, wantedHeight, heightDamping * Time.deltaTime);

		//        // Convert the angle into a rotation
		//        var currentRotation = Quaternion.Euler(0, currentRotationAngle, 0);

		// Set the position of the camera on the x-z plane to the position of the user:
		transform.position = user.position + user.forward * distance;

		// Set the height of the camera
		transform.position = new Vector3 (transform.position.x, currentHeight, transform.position.z);

		// Always look at the target's head
		if (target != null) {
			transform.LookAt (target.position + user.up * 5.0f);
		} else {
			// If there is no target yet, look at a bit forward of the camera/user position.
			transform.LookAt (user.position + user.forward * distance * 2 + user.up * 5.0f);
		}
	}
}
