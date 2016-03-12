using UnityEngine;
using System.Collections;

public class PlayerToolPack : MonoBehaviour
{

    private bool hasResizeTool;
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
        if (collision.collider.tag == "ResizeTool")
        {
            hasResizeTool = true;
        }
    }

    // When leaving this tool, the player can no longer click and use this tool.
    // This guarantees the player to use the tool only if they are near the tool.
    public void ClearResizeTool()
    {
        hasResizeTool = false;
    }

    public bool HasResizeTool()
    {
        return hasResizeTool;
    }
}
