using UnityEngine;
using System.Collections;

public interface ToolInterface
{
    // Try to use this tool.
    // player starts using the tool.
    void Use(Transform player);
    // Change back to normal view if the tool was in use.
    void Done();
    // SetTarget.
    void SetTarget(Transform target);
    // Perform what this tool is built for.
    void TryPerform(float input);
    // Returns a ToolStatus that contains all tool related parameters.
    // Definition can be found in Struct.cs
    ToolStatus GetStatus();
}

