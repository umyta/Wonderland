using UnityEngine;
using System.Collections;

public struct RayCastReturnValue
{
    public GameObject hitObject;
    public Vector3 hitPoint;
}

public struct ToolStatus
{
    public float factor;
    public Transform userTransform;
    public Transform targetTransform;
}