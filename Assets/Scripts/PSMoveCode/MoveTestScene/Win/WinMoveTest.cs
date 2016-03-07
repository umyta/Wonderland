using UnityEngine;
using System.Collections;
using MoveServerNS;

public class WinMoveTest : MonoBehaviour {

    public WinMoveServer moveServer;
    // Use this for initialization

    [Range(1, 6)]
    public int controller;

    private int r = 0;
    private int g = 255;
    private int b = 0;
	void Start () {
        //using only one controller as default
        controller = 1;
	}
	
	// Update is called once per frame
	void Update () {
        WinMoveController move = moveServer.getController(controller-1);
        if (move != null)
        {
            if (move.btnOnPress(MoveButton.BTN_MOVE))
            {
                r = (int)(Random.value * 255);
                g = (int)(Random.value * 255);
                b = (int)(Random.value * 255);
                moveServer.Send_setMoveLight(move, r, g, b);
            }
            if (move.btnOnPress(MoveButton.BTN_SQUARE))
            {
                r = 0;
                moveServer.Send_setMoveLight(move, r, g, b);
            }
            if (move.btnOnPress(MoveButton.BTN_TRIANGLE))
            {
                print("Triangle");
                moveServer.Send_calibrateOrientation(move);
            }
            if (move.btnOnPress(MoveButton.BTN_CROSS))
            {
                moveServer.Send_resetMoveLight(move);
            }
            //tr
            transform.rotation = move.getQuaternion();
            transform.position = new Vector3(move.getPositionSmooth().x*10, move.getPositionSmooth().y * 10, move.getPositionSmooth().z * 10);
            if (move.triggerValue > 50)
            {
                moveServer.Send_rumbleController(move, move.triggerValue);
            }
        }
	}
}
