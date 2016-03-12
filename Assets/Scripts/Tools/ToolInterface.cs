using UnityEngine;
using System.Collections;

public interface ToolInterface
{
    // Try to use this tool.
    // Checks if the user has the right to use this tool, and start using the tool.
    void TryUse();
    // Change back to normal view if the tool was in use.
    void Done();
}

