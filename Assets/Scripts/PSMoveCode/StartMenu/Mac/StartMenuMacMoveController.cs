using UnityEngine;
using System.Collections;

public class StartMenuMacMoveController : MonoBehaviour
{
    private Transform moveController;
    void Awake()
    {
        Transform[] trs = GetComponentsInChildren<Transform>();
        //For one controller
        if (trs.Length > 0) {
            moveController = trs[0];
        }
    }
}
