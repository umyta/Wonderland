using UnityEngine;
using System.Collections;
using MoveServerNS;

public class StartMenuWinController : MonoBehaviour {
    //Sony move server
    public WinMoveServer moveServer;

    //controller
    Vector3 oldPos;

    [Range(1, 6)]
    public int controller;

    //raycast control animation
    private bool raycastLastHit = false;
    private GameObject lastHitObject = null;

    void Start()
    {
        //using only one controller as default
        controller = 1;
        transform.Rotate(0f, -180f, 0f);
    }

    // Update is called once per frame
    void Update()
    {
        //update game object location
        WinMoveController move = moveServer.getController(controller - 1);
        if (move != null)
        {
            //transform
            Vector3 currPos = new Vector3(move.getPositionSmooth().x * 50, move.getPositionSmooth().y * 50, 0);
            Vector3 delta = currPos - oldPos;
            transform.position = transform.position + delta;
            oldPos = currPos;

            //ray cast
            RaycastHit hit;
            Vector3 destination = transform.position - Camera.main.transform.position;
            Debug.DrawLine(transform.position, destination);
            
            if (Physics.Raycast(transform.position, destination, out hit))
            {
                GameObject hitObject = hit.transform.gameObject;
                //Enter button pressed
                if (hitObject.name == "EnterBut" && move.btnOnPress(MoveButton.BTN_MOVE))
                {
                    hitObject.GetComponent<Enter>().enterScene();
                } else if (hitObject.name == "EnterBut" && !raycastLastHit) {
                    //First time hitting Enter button
                    raycastLastHit = true;
                    lastHitObject = hitObject;
                    //Button animation
                    hitObject.GetComponent<Enter>().setEnterAnimationTrue();
                    Debug.Log("I hit enter button");
                }
            }
            //Last hit an enter button, exit animation
            else if (raycastLastHit) {
                lastHitObject.GetComponent<Enter>().setExitAnimationTrue();
                raycastLastHit = false;
                lastHitObject = null;
            }
        }
    }
}
