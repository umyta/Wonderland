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
    int UIMask;
    float cameraRayLength = 100f;
    bool menuActive;

    void Awake()
    {
        floorMask = LayerMask.GetMask("Floor");
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

        //detect menu activation
        if (Input.GetMouseButtonDown(0))
        {
            openIngameUI();
        }

        //if menu active, only allow menu operations
        if (menuActive)
        {
            menuHighlight();
        }
        else
        {
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

    void menuHighlight()
    {
        GameObject[] menuItems = GameObject.FindGameObjectsWithTag("UI");
        foreach (GameObject menuItem in menuItems)
        {
            MeshRenderer mesh = menuItem.transform.GetComponentInChildren<MeshRenderer>();
            mesh.material.color = Color.white;
        }
        //Highlight hover items
        Ray camRay = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit buttonHit = new RaycastHit();
        if (Physics.Raycast(camRay, out buttonHit, cameraRayLength, UIMask))
        {
            MeshRenderer mesh = buttonHit.transform.GetComponentInChildren<MeshRenderer>();
            mesh.material.color = Color.yellow;
            Debug.Log("Hit a " + buttonHit.transform.gameObject.name);
            if (buttonHit.transform.gameObject.name == "Exit")
            {
                buttonHit.transform.GetComponent<doorAnimation>().setAnimationActive();
            }
        }
    }

    void openIngameUI()
    {
        Ray camRay = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit playerHit = new RaycastHit();
        if (Physics.Raycast(camRay, out playerHit, cameraRayLength))
        {
            if (playerHit.transform.gameObject.tag == "Player")
            {
                Debug.Log("Clicking on player. Should open menu...");
                menuActive = !menuActive;
                Camera.main.transform.Find("UI").GetComponent<UI>().setActive(menuActive);
            }
        }
    }

    void Animating(float h, float v)
    {
        // If either h or v is not 0, the player is walking. 
        bool walking = h != 0f || v != 0f;
        anim.SetBool("IsWalking", walking); 
    }
}
