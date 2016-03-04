using UnityEngine;
using System.Collections;

/**
 * This script ensures the camera to follow the player with a smooth transition.
 */
public class CameraFollow : MonoBehaviour
{
    Transform target;
    public float smoothing = 5f;

    Vector3 offset;

    void Start()
    {
        offset = transform.position - Vector3.zero;
    }

    public void SetTarget(Transform target)
    {
        this.target = target;
        offset = transform.position - this.target.position;
    }

    // Player has rigid body, so we are following using fixedupdate as well.
    void FixedUpdate()
    {
        if (target == null)
        {
            return;
        }

        Vector3 targetCamPos = target.position + offset;
        transform.position = 
            Vector3.Lerp(
            transform.position, 
            targetCamPos, 
            smoothing * Time.deltaTime);
    }
}
