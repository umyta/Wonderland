using UnityEngine;
using System.Collections;

public class Level01Platform : MonoBehaviour {
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
        Debug.Log("Starting");
        isWin = isMac = initialized = false;
        if (Application.platform == RuntimePlatform.WindowsEditor ||
            Application.platform == RuntimePlatform.WindowsPlayer)
        {
            Debug.Log("Windows System");
            isWin = true;
            WinServerInstance = Instantiate(WinServerPrefab, new Vector3(0f, 0f, 0f), Quaternion.identity) as GameObject;
        }
        else if (Application.platform == RuntimePlatform.OSXEditor ||
            Application.platform == RuntimePlatform.OSXPlayer)
        {
            Debug.Log("Mac OS");
            isMac = true;
            //Instantiate(MacServer, new Vector3(0f, 0f, 0f), Quaternion.identity);
            //Instantiate(MacController, new Vector3(0f, 0f, 0f), Quaternion.identity);
        }
    }

    void Update() {
        //Windows
        if (!initialized) {
            if (isWin)
            {
                WinControllerInstance = GameObject.FindGameObjectWithTag("Player");
                if (WinControllerInstance != null)
                {
                    initialized = true;
                    WinControllerInstance.AddComponent<Level01WinController>();
                    WinControllerInstance.GetComponent<Level01WinController>().moveServer = WinServerInstance.transform.GetComponent<MoveServerNS.WinMoveServer>();
                }
            }
            else if (isMac) {
                //Do something
            }                    
        }       
    }
}
