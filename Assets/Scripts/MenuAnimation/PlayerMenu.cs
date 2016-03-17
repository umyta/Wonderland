using UnityEngine;
using System.Collections;

public class PlayerMenu : MonoBehaviour
{
    //Transform menu items
    private Transform LightObj;
    private Transform ExitObj;
    private Transform StarObj;
    private Transform ToolObj;
    private Transform KeyObj;
    private Transform Clue1;
    private Transform Clue2;
    private Transform Clue0;

    //Destination
    private Vector3 D1 = new Vector3(-5f, 0f, 0f);
    private Vector3 D2 = new Vector3(-2f, 0f, 0f);
    private Vector3 D3 = new Vector3(2f, 0f, 0f);
    private Vector3 D4 = new Vector3(5f, 0f, 0f);
    private Vector3 startLocation = new Vector3(0f, 0f, 0f);

    //Animation
    private float count = 0;
    private bool isOpenAnimationActive = false;
    private bool isCloseAnimationActive = false;

    // Use this for initialization
    void Awake()
    {
        LightObj = transform.Find("Spotlight");
        ExitObj = transform.Find("Exit");
        StarObj = transform.Find("Clue");
        ToolObj = transform.Find("Tool");
        KeyObj = transform.Find("Key");
        Clue1 = transform.Find("clue_1");
        Clue2 = transform.Find("clue_2");
        Clue0 = transform.Find("clue_none");
    }
	
    // Update is called once per frame
    void Update()
    {
        if (LightObj == null) {
            LightObj = transform.Find("Spotlight");
            ExitObj = transform.Find("Exit");
            StarObj = transform.Find("Clue");
            ToolObj = transform.Find("Tool");
            KeyObj = transform.Find("Key");
            Clue1 = transform.Find("clue_1");
            Clue2 = transform.Find("clue_2");
            Clue0 = transform.Find("clue_none");
        }
        OpenMenu();
        CloseMenu();
    }

    //not enabled right now
    public void CloseMenu()
    {
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

    //animation to open the menu
    public void OpenMenu()
    {
        if (isOpenAnimationActive)
        {
            if (count > 1.0f)
            {
                count = 0.0f;
                isOpenAnimationActive = false;
            }
            else
            {
                ExitObj.localPosition = Vector3.Lerp(startLocation, D1, count);
                StarObj.localPosition = Vector3.Lerp(startLocation, D2, count);
                ToolObj.localPosition = Vector3.Lerp(startLocation, D3, count);
                KeyObj.localPosition = Vector3.Lerp(startLocation, D4, count);
                count += 0.1f;
            }
        }
    }

    //set menu items active or inactive
    public void SetActive(bool isActive)
    {
        Debug.Log("Menu active set to " + isActive);
        LightObj.gameObject.SetActive(isActive);
        ExitObj.gameObject.SetActive(isActive);
        ToolObj.gameObject.SetActive(isActive);
        KeyObj.gameObject.SetActive(isActive);
        StarObj.gameObject.SetActive(isActive);
        Clue1.gameObject.SetActive(false);
        Clue2.gameObject.SetActive(false);
        Clue0.gameObject.SetActive(false);

        if (isActive)
        {
            count = 0.0f;
            isOpenAnimationActive = true;
        }
        //close animation is not enabled right now
        else
        {
            count = 0.0f;
            isCloseAnimationActive = true;
        }
    }

    public void displayClue(ClueState clueState)
    {
        if (clueState == ClueState.None)
            Clue0.gameObject.SetActive(!Clue0.gameObject.activeSelf);
        else if (clueState == ClueState.Clue1)
            Clue1.gameObject.SetActive(!Clue1.gameObject.activeSelf);
        else if (clueState == ClueState.Clue2)
            Clue2.gameObject.SetActive(!Clue2.gameObject.activeSelf);
    }
}
