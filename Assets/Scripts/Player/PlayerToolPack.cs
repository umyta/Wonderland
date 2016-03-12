using UnityEngine;
using System.Collections;

public class PlayerToolPack : MonoBehaviour
{

    private Transform resizeTool;
    private Transform magnetTool;
    private Transform springTool;
    // Use this for initialization
    void Start()
    {
    }
	
    // Update is called once per frame
    void Update()
    {
	
    }

    void OnCollisionEnter(Collision collision)
    {
        Transform tool = collision.collider.transform;

        string colliderTag = tool.tag;
        if (colliderTag == "ResizeTool")
        {
            resizeTool = tool;
        }
        else if (colliderTag == "MagnetTool")
        {
            magnetTool = tool;
        }
        else if (colliderTag == "SpringTool")
        {
            springTool = tool;
        } 
    }

    // When leaving this tool, the player can no longer click and use this tool.
    // This guarantees the player to use the tool only if they are near the tool.
    public void ClearResizeTool()
    {
        resizeTool = null;
    }

    public bool HasResizeTool()
    {
        return resizeTool != null;
    }

    public Transform UseResizeTool()
    {
        return resizeTool;
    }

    public void ClearMagnetTool()
    {
        magnetTool = null;
    }

    public bool HasMagnetTool()
    {
        return magnetTool != null;
    }

    public Transform UseMagnetTool()
    {
        return magnetTool;
    }

    public void ClearSpringTool()
    {
        springTool = null;
    }

    public bool HasSpringTool()
    {
        return springTool != null;
    }

    public Transform UseSpringTool()
    {
        return springTool;
    }
}
