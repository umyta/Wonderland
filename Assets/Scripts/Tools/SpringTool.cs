using UnityEngine;
using System.Collections;

public class SpringTool : MonoBehaviour, ToolInterface
{
    public void Use(Transform player)
    {
        Debug.Log("Try to use Spring Tool");
    }

    public void Done()
    {
        Debug.Log("Done using Spring Tool");
    }

    public void SetTarget(Transform target)
    {
    }

    public void TryPerform(float force)
    {
    }

    public ToolStatus GetStatus()
    {
        return new ToolStatus();
    }
}
