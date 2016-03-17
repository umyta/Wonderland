using UnityEngine;
using System.Collections;

public class Scaler : MonoBehaviour {
    private Transform scale_front;
    private Vector3 initScale;

	// Use this for initialization
	void Awake () {
        scale_front = transform.FindChild("scale_front");
        initScale = scale_front.localScale;
	}

    public void scale(float scale)
    {
        scale_front.localScale = Vector3.Lerp(scale_front.localScale, initScale * scale, Time.deltaTime);
    }
}
