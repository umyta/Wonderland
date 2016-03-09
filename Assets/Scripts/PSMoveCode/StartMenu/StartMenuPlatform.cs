using UnityEngine;
using System.Collections;

public class StartMenuPlatform : MonoBehaviour
{
    //Windows controllers
    public GameObject WinServer;
    public GameObject WinController;
    private GameObject WinServerInstance;
    private GameObject WinControllerInstance;

    //Mac controllers
    public GameObject MacServer;
    public GameObject MacController;

    // Use this for initialization
    void Start()
    {
        Debug.Log("Starting");
        if (Application.platform == RuntimePlatform.WindowsEditor ||
            Application.platform == RuntimePlatform.WindowsPlayer)
        {
            Debug.Log("Windows System");
            WinServerInstance = Instantiate(WinServer, new Vector3(0f, 0f, 0f), Quaternion.identity) as GameObject;
            WinControllerInstance = Instantiate(WinController, new Vector3(0f, 0f, 0f), Quaternion.identity) as GameObject;
            WinControllerInstance.AddComponent<StartMenuWinController>();
            WinControllerInstance.GetComponent<StartMenuWinController>().moveServer = WinServerInstance.transform.GetComponent<MoveServerNS.WinMoveServer>();
        }
        else if (Application.platform == RuntimePlatform.OSXEditor ||
                 Application.platform == RuntimePlatform.OSXPlayer)
        {
            Debug.Log("Mac OS");
            Instantiate(MacController, new Vector3(0f, 0f, 0f), Quaternion.identity);
            Instantiate(MacServer, new Vector3(0f, 0f, 0f), Quaternion.identity);
        }
    }
}