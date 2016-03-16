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
}
