using UnityEngine;
using System.Collections;

public class UI : MonoBehaviour {
    //Transform menu items
    private Transform LightObj;
    private Transform ExitObj;
    private Transform StarObj;
    private Transform ToolObj;
    private Transform KeyObj;

    private bool isActive;

    //Destination
    private Vector3 D1 = new Vector3 (-5f, 0f, 0f);
    private Vector3 D2 = new Vector3(-2f, 0f, 0f);
    private Vector3 D3 = new Vector3(2f, 0f, 0f);
    private Vector3 D4 = new Vector3(5f, 0f, 0f);
    private Vector3 startLocation = new Vector3(0f, 0f, 0f);

    //Animation
    private float count = 0;
    private bool isOpenAnimationActive = false;
    private bool isCloseAnimationActive = false;

	// Use this for initialization
	void Awake () {
        LightObj = transform.Find("Spotlight");
        ExitObj = transform.Find("Exit");
        StarObj = transform.Find("Clue");
        ToolObj = transform.Find("Tool");
        KeyObj = transform.Find("Key");
	}
	
	// Update is called once per frame
	void Update () {
        openMenu();
        closeMenu();
	}

    public void closeMenu() {
        if (isCloseAnimationActive)
        {
            if (count > 1.0f)
            {
                count = 0.0f;
                isCloseAnimationActive = false;
            }
            else
            {
                ExitObj.localPosition = Vector3.Lerp(D1, startLocation, count);
                StarObj.localPosition = Vector3.Lerp(D2, startLocation, count);
                ToolObj.localPosition = Vector3.Lerp(D3, startLocation, count);
                KeyObj.localPosition = Vector3.Lerp(D4, startLocation, count);
                count += 0.1f;
            }
        }
    }

    public void openMenu() {
        if (isOpenAnimationActive) {
            if (count > 1.0f)
            {
                count = 0.0f;
                isOpenAnimationActive = false;
            }
            else {
                ExitObj.localPosition = Vector3.Lerp(startLocation, D1, count);
                StarObj.localPosition = Vector3.Lerp(startLocation, D2, count);
                ToolObj.localPosition = Vector3.Lerp(startLocation, D3, count);
                KeyObj.localPosition = Vector3.Lerp(startLocation, D4, count);
                count += 0.1f;
            }
        }
    }

    public void setActive(bool isActive) {
        this.isActive = isActive;
        LightObj.gameObject.SetActive(isActive);
        ExitObj.gameObject.SetActive(isActive);
        ToolObj.gameObject.SetActive(isActive);
        KeyObj.gameObject.SetActive(isActive);
        StarObj.gameObject.SetActive(isActive);

        if (isActive)
        {
            count = 0.0f;
            isOpenAnimationActive = true;
        }
        else {
            count = 0.0f;
            isCloseAnimationActive = true;
        }
    }
}
