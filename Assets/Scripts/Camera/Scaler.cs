using UnityEngine;
using System.Collections;

public class Scaler : MonoBehaviour {
    private Transform scale_front;

	// Use this for initialization
	void Awake () {
        scale_front = transform.FindChild("scale_front");
	}

    public void scale(float scale)
    {
        scale_front.localScale = scale_front.localScale * scale;
    }
}
