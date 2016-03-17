using UnityEngine;
using System.Collections;

public class Scaler : MonoBehaviour
{
    public Transform scale_front;
    private Vector3 initScale;

    // Use this for initialization
    void Awake()
    {
        initScale = scale_front.localScale;
    }

    public void scale(float scale)
    {
        scale_front.localScale = Vector3.Lerp(scale_front.localScale, initScale * scale, Time.deltaTime);
    }
}
