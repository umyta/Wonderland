using UnityEngine;
using System.Collections;
using MoveServerNS;

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
        Vector2 cursorPt2D = new Vector2(cursorScreenPt.x ,cursorScreenPt.y);
        Ray ray = cam.ScreenPointToRay(cursorPt2D);
        Debug.DrawRay(ray.origin, ray.direction.normalized * cameraRayLength, Color.green, Time.deltaTime, true);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, camRayLength, layermask)) {
            return hit.collider.gameObject;
        }
        return null;
    }

    // If there is at least one controller detected by this server.
    public static bool HasMoveControllers(WinMoveServer moveServer)
    {
        return moveServer.getController(0) != null;
    }

    public static OperatingSystemForController GetOS()
    {
        if (Application.platform == RuntimePlatform.WindowsEditor ||
            Application.platform == RuntimePlatform.WindowsPlayer)
        {
            return OperatingSystemForController.Windows;
        }
        if (Application.platform == RuntimePlatform.OSXEditor ||
            Application.platform == RuntimePlatform.OSXPlayer)
        {
            return OperatingSystemForController.Mac;
        }

        return OperatingSystemForController.UnknownSystem;
    }
}
