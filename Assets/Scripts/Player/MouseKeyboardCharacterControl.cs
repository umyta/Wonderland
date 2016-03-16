using UnityEngine;

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(MenuUI))]
public class MouseKeyboardCharacterControl : MonoBehaviour
{
    // Stores animation states for serialization.
    public enum PlayerState
    {
        Idle,
        Walking}

    ;

    public bool isControllable = false;
    public float speed = 6f;
    public PlayerState state;

    Vector3 movement;
    Animator anim;
    Rigidbody playerRigidbody;
    int floorMask;

    MenuUI menu;

    void Awake()
    {
        floorMask = LayerMask.GetMask("Floor");
        anim = GetComponent<Animator>();
        playerRigidbody = GetComponent<Rigidbody>();
        menu = GetComponent<MenuUI>();
    }

    // Called for every physics update.
    void FixedUpdate()
    {
        // Only detects mouse movement if 
        if (isControllable)
        {
            // Raw value will have -1, 0 or 1. Full speed right away more responsive.
            float h = Input.GetAxisRaw("Horizontal"); 
            float v = Input.GetAxisRaw("Vertical");

            // Only give controls to movement when menu is not active.
            if (!menu.isActive)
            {
                //player is allowed to move
                Move(h, v);
                Turning();
                Animating(h, v);
            }
        }
    }

    void Move(float h, float v)
    {
        movement = transform.forward * v;
        movement += transform.right * h;
        movement = movement.normalized * speed * Time.deltaTime;
        playerRigidbody.MovePosition(transform.position + movement);
    }

    void Turning()
    {
        RayCastReturnValue hitValue = HelperLibrary.RaycastObject(Input.mousePosition, floorMask);
        if (hitValue.hitObject != null)
        {
            Vector3 playerToMouse = hitValue.hitPoint - transform.position;
            playerToMouse.y = 0.0f; // Make sure that they player does not move away from the floor
            Quaternion newRotation = Quaternion.LookRotation(playerToMouse * Time.deltaTime);
            playerRigidbody.MoveRotation(newRotation);
        }
    }

    void Animating(float h, float v)
    {
        // If either h or v is not 0, the player is walking. 
        bool isWalking = h != 0f || v != 0f;
        if (isWalking)
        {
            state = PlayerState.Walking;   
        }
        else
        {
            state = PlayerState.Idle;
        }
        SetAnimationState(state);
    }

    // Set triggers for animation.
    public void SetAnimationState(PlayerState state)
    {
        if (state == PlayerState.Walking)
        {
            anim.SetBool("IsWalking", true); 
        }
        else
        {
            anim.SetBool("IsWalking", false);
        }

    }
}
