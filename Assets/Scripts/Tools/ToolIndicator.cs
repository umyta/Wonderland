using UnityEngine;
using System.Collections;

public class ToolIndicator : MonoBehaviour {
    private float scale = 1.0f;
    private bool up = true;
	
	// Update is called once per frame
	void Update () {
        if (scale > 1.1f)
            up = false;
        else if (scale < 0.9f)
            up = true;

        if (up)
        {
            scale += 0.01f;
            transform.localScale = transform.localScale * 1.001f;
        }
        else
        {
            scale -= 0.01f;
            transform.localScale = transform.localScale * 0.999f;
        }
    }
}
