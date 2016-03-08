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
	}

    public void openMenu() {
        ExitObj.localPosition = D1;
        StarObj.localPosition = D2;
        ToolObj.localPosition = D3;
        KeyObj.localPosition = D4;
    }

    public void setActive(bool isActive) {
        this.isActive = isActive;
        LightObj.gameObject.SetActive(isActive);
        ExitObj.gameObject.SetActive(isActive);
        ToolObj.gameObject.SetActive(isActive);
        KeyObj.gameObject.SetActive(isActive);
        StarObj.gameObject.SetActive(isActive);
    }
}
