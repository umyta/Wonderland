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
    public void PlayerFollowTarget(Transform user, Transform target)
    {
        if (user == null)
        {
            return;
        }
        transform.position = user.position + user.up * height + user.forward * distance;
        transform.rotation = user.rotation;
    }
}
