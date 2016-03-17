using UnityEngine;
using System.Collections;


public class HelperLibrary
{
    public static float cameraRayLength = 100f;
    // Raycast helper function returns an object.
    public static RayCastReturnValue RaycastObject(Vector2 screenPos, int layermask = Physics.DefaultRaycastLayers)
    {
        RaycastHit info;
        RayCastReturnValue raycastRetVal = new RayCastReturnValue();
        if (Physics.Raycast(Camera.main.ScreenPointToRay(screenPos), out info, cameraRayLength, layermask))
        {
            raycastRetVal.hitObject = info.collider.gameObject;
            raycastRetVal.hitPoint = info.point;
            return raycastRetVal;
        }
        else
        {
            raycastRetVal.hitObject = null;
            raycastRetVal.hitPoint = Vector3.zero;
        }

        return raycastRetVal;
    }

    public static GameObject WorldToScreenRaycast(Vector3 pos, Camera cam, int camRayLength, int layermask = Physics.DefaultRaycastLayers)
    {
        Vector3 cursorScreenPt = cam.WorldToScreenPoint(pos);
        Ray ray = cam.ScreenPointToRay(cursorScreenPt);
        Debug.DrawRay(ray.origin, ray.direction, Color.green, Time.deltaTime, true);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, camRayLength, layermask)) {
            return hit.transform.gameObject;
        }
        return null;
    }
}
