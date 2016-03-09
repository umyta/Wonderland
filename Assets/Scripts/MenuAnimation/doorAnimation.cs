using UnityEngine;
using System.Collections;

public class doorAnimation : MonoBehaviour {
    private bool isActive;
    private bool setInitial;
    private Quaternion originalRot;
    private float count;

	// Use this for initialization
	void Start () {
	    
	}
	
	// Update is called once per frame
	void Update () {
        Transform innerDoor = transform.FindChild("door");
        if (innerDoor != null && setInitial != true) {
            originalRot = innerDoor.localRotation;
            setInitial = true;
        }

        if (isActive)
        {
            openAnimation();
        }
        else {
            reset();
        }
	}

    public void openAnimation() {
        Transform innerDoor = transform.FindChild("door");
        if (innerDoor != null)
        {
            if (count > 45.0f)
            {
                return;
            }
            else {
                innerDoor.Rotate(0f, 2f, 0f);
                count += 1f;
            }
        }
    }

    public void setAnimationActive() {
        isActive = true;
    }

    public void reset() {
        Transform innerDoor = transform.FindChild("door");
        innerDoor.localRotation = originalRot;
        count = 0.0f;
    }
}
