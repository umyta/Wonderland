using UnityEngine;

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(Rigidbody))]
public class PlayerMovement : MonoBehaviour
{
    public float speed = 6f;
    Vector3 movement;
    Animator anim;
    Rigidbody playerRigidbody;
    int floorMask;
    int playerMask;
    int UIMask;
    float cameraRayLength = 100f;
    bool menuActive;

    void Awake()
    {
        floorMask = LayerMask.GetMask("Floor");
        playerMask = LayerMask.GetMask("Player");
        UIMask = LayerMask.GetMask("UI");
        anim = GetComponent<Animator>();
        playerRigidbody = GetComponent<Rigidbody>();
    }

    // Called for every physics update.
    void FixedUpdate()
    {
        // Raw value will have -1, 0 or 1. Full speed right away more responsive.
        float h = Input.GetAxisRaw("Horizontal"); 
        float v = Input.GetAxisRaw("Vertical"); 
        Move(h, v);
        Turning();
        openIngameUI();
        Animating(h, v);
    }

    void Move(float h, float v)
    {
        movement.Set(h, 0f, v);
        movement = movement.normalized * speed * Time.deltaTime;
        playerRigidbody.MovePosition(transform.position + movement);
    }

    void Turning()
    {
        Ray camRay = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit floorHit;
        if (Physics.Raycast(camRay, out floorHit, cameraRayLength, floorMask))
        {
            Vector3 playerToMouse = floorHit.point - transform.position;
            playerToMouse.y = 0.0f; // Make sure that they player does not move away from the floor
            Quaternion newRotation = Quaternion.LookRotation(playerToMouse * Time.deltaTime);
            playerRigidbody.MoveRotation(newRotation);
        }
    }

    void openIngameUI() {
        Ray camRay = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(camRay, cameraRayLength, playerMask)) {
            menuActive = !menuActive;
            Camera.main.transform.Find("UI").GetComponent<UI>().setActive(menuActive);
        }
    }

    void Animating(float h, float v)
    {
        // If either h or v is not 0, the player is walking. 
        bool walking = h != 0f || v != 0f;
        anim.SetBool("IsWalking", walking); 
    }
}
