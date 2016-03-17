using UnityEngine;
using System.Collections;

public class Scaler : MonoBehaviour
{
    private Transform scale_back;
    private Transform scale_front;
    private Transform NumDisplay;
    private Vector3 sf_initScale;

    // Use this for initialization
    void Awake()
    {
        scale_back = transform.Find("scale_back");
        scale_front = transform.Find("scale_front");
        NumDisplay = transform.Find("NumDisplay");
        sf_initScale = scale_front.localScale;
    }

    public void setActive(bool active) {
        scale_back.gameObject.SetActive(active);
        scale_front.gameObject.SetActive(active);
        NumDisplay.gameObject.SetActive(active);
        scale_front.localScale = sf_initScale;
    }

    public void scale(float scale)
    {
        TextMesh textMesh = NumDisplay.GetComponent<TextMesh>();
        textMesh.text = scale.ToString("F2");
        scale_front.localScale = Vector3.Lerp(scale_front.localScale, sf_initScale * scale, Time.deltaTime);
    }
}
