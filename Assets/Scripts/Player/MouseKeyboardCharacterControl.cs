using UnityEngine;


[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(MenuUI))]
public class MouseKeyboardCharacterControl : MonoBehaviour
{

    public enum RotationAxes
    {
        MouseXAndY = 0,
        MouseX = 1,
        MouseY = 2

    }

    public RotationAxes axes = RotationAxes.MouseXAndY;
    public float sensitivityX = 15F;
    public float sensitivityY = 15F;

    public float minimumX = -360F;
    public float maximumX = 360F;

    public float minimumY = -60F;
    public float maximumY = 60F;

    float rotationY = 0F;

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

    void Start()
    {
    }
    // Called for every physics update.
    void FixedUpdate()
    {
        // Only detects mouse movement if 
        if (isControllable && !menu.isActive)
        {   // Only give controls to movement when menu is not active.
            // Raw value will have -1, 0 or 1. Full speed right away more responsive.
            float h = Input.GetAxisRaw("Horizontal"); 
            float v = Input.GetAxisRaw("Vertical");
            //player is allowed to move
            Move(h, v);
            Turning();
            Animating(h, v);
        }
    }

    void Move(float h, float v)
    {
        movement = transform.forward * v;
        movement += transform.right * h;
        if (movement != Vector3.zero)
        {
//            Debug.Log("Moving" + movement.ToString());
            movement = movement.normalized * speed * Time.deltaTime;
            playerRigidbody.MovePosition(transform.position + movement);
        }
    }

    void Turning()
    {
        if (axes == RotationAxes.MouseXAndY)
        {
            float rotationX = transform.localEulerAngles.y + Input.GetAxis("Mouse X") * sensitivityX;

            rotationY += Input.GetAxis("Mouse Y") * sensitivityY;
            rotationY = Mathf.Clamp(rotationY, minimumY, maximumY);

            transform.localEulerAngles = new Vector3(-rotationY, rotationX, 0);
        }
        else if (axes == RotationAxes.MouseX)
        {
            transform.Rotate(0, Input.GetAxis("Mouse X") * sensitivityX, 0);
        }
        else
        {
            rotationY += Input.GetAxis("Mouse Y") * sensitivityY;
            rotationY = Mathf.Clamp(rotationY, minimumY, maximumY);

            transform.localEulerAngles = new Vector3(-rotationY, transform.localEulerAngles.y, 0);
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
