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
    void Perform(float input);

}

