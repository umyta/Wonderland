using UnityEngine;
using System.Collections;

public class Level01Platform : MonoBehaviour
{
    //Windows controllers
    public GameObject WinServerPrefab;
    private GameObject WinServerInstance;
    private GameObject WinControllerInstance;
    private bool isWin;

    //Mac controllers
    //public GameObject MacServer;
    //public GameObject MacController;
    private bool isMac;

    //Initialized
    private bool initialized;

    // Use this for initialization
    void Start()
    {
        switch (HelperLibrary.GetOS())
        {
            case OperatingSystemForController.Mac:
                isMac = true;
                Debug.Log("Detected Mac System, please use mouse control");
                break;
            case OperatingSystemForController.Windows:
                WinServerInstance = Instantiate(WinServerPrefab, new Vector3(0f, 0f, 0f), Quaternion.identity) as GameObject;
                isWin = true;
                break;
        }

    }

    void Update()
    {
        //Windows
        if (!initialized)
        {
            if (isWin)
            {
                WinControllerInstance = GameObject.FindGameObjectWithTag("Player");
                if (WinControllerInstance != null)
                {
                    initialized = true;
                    WinControllerInstance.AddComponent<Level01WinController>();
                    WinControllerInstance.GetComponent<Level01WinController>().SetMoveServer(WinServerInstance.transform.GetComponent<MoveServerNS.WinMoveServer>());
                }
            }
            else if (isMac)
            {
                //Do something
            }                    
        }       
    }
}
