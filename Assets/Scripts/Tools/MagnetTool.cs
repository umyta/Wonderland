using UnityEngine;
using System.Collections;

public class MagnetTool : MonoBehaviour, ToolInterface
{
    public void Use(Transform player)
    {
        Debug.Log("Try to use Magnet Tool");
    }

    public void Done()
    {
        Debug.Log("Done using Magnet Tool");
    }
}
