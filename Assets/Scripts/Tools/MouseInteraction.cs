using UnityEngine;
using System.Collections;

/**
 * This is used to interact with the environment.
 * Attach this to a specific environment object in order to interact with them
 * individually.
 */
public class MouseInteraction : MonoBehaviour
{
    // A tool can be any of the defined tool script that implement ToolInterface.
    // e.g. ResizeTool, MagnetTool, SpringTool.
    private ToolInterface tool;
    // Use this for initialization
    void Start()
    {
        tool = GetComponent<ToolInterface>();
    }


    void OnMouseDown()
    {
        // If this item has been clicked, try to use this for resizing.
        if (tool != null)
        {
            tool.TryUse();    
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Home))
        {
            if (tool != null)
            {
                tool.Done();
            }
        }
    }
}
