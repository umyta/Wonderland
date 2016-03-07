using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class Enter : MonoBehaviour {
    private float counter = 0.0f;
    private bool enterActive;
    private bool exitActive;

    void Update() {
        if (enterActive)
        {
            enterAnimation();
        }
        else if (exitActive) {
            exitAnimation();
        }
    }

    public void enterScene() {
        SceneManager.LoadScene("Level 01");
    }

    public void enterAnimation() {
        if (counter > 1.0f)
        {
            enterActive = false;
            counter = 0.0f;
        }
        else {
            Debug.Log("Enter animation");
            transform.localScale = transform.localScale + new Vector3(0.1f, 0.1f, 0.1f);
            counter += 0.2f;
        }
    }

    public void setEnterAnimationTrue() {
        enterActive = true;
        counter = 0.0f;
    }

    public void exitAnimation() {
        if (counter > 1.0f)
        {
            exitActive = false;
            counter = 0.0f;
        }
        else
        {
            transform.localScale = transform.localScale - new Vector3(0.1f, 0.1f, 0.1f);
            counter += 0.2f;
        }
    }

    public void setExitAnimationTrue() {
        exitActive = true;
        counter = 0.0f;
    }
}
