using UnityEngine;
using System.Collections;

public class MagnetTool : MonoBehaviour, ToolInterface
{

    // Use this for initialization
    void Start()
    {
	
    }
	
    // Update is called once per frame
    void Update()
    {
	
    }

    public void TryUse()
    {
        Debug.Log("Try to use Magnet Tool");
    }

    public void Done()
    {
        Debug.Log("Done using Magnet Tool");
    }
}
