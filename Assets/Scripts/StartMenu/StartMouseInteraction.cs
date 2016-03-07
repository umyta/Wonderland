using UnityEngine;
using System.Collections;

public class StartMouseInteraction : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
        if (Input.GetMouseButtonDown(0)) {
            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out hit, 100)) {
                GameObject hitObject = hit.transform.gameObject;
                Debug.Log(hitObject.name);
                switch (hitObject.name) {
                    case "EnterBut":
                        hitObject.GetComponent<Enter>().enterScene();
                        break;
                }
            }
        }
	}
}
