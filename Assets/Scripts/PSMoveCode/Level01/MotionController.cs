using UnityEngine;
using System.Collections;

/* This is an interface for motion controller of all operating systems */
public interface MotionController {
    //Physics rigidbody update
    void FixedUpdate();
    //Initialize positions
    void setInitialPos(Vector3 pos, Quaternion angle, Vector3 initialPos, Quaternion initialRot, bool setInitial);
    //Move on floor
    void Move(Vector3 deltaPos);
    //Rotational turn
    void Turning(Vector3 deltaPos);
    //animation
    void Animating(Vector3 deltaPos);
}
