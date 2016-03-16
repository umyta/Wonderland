using UnityEngine;
using System.Collections;

[RequireComponent((typeof(PhotonView)))]
public class ResizeTool : MonoBehaviour, ToolInterface
{
    // The user is the one that controls the camera
    public Transform cameraToolTransform;

    private Camera standbyCamera;
    private Camera cameraTool;
    private Transform user;
    private Transform target;
    private Collider[] colliders;
    // The distance in the x-z plane to the target
    public float distance = 5.0f;
    // the height we want the camera to be above the target
    public float height = 5.0f;
    public float heightDamping = 2.0f;
    // How fast does this camera turn.
    public float rotationDamping = 0.1f;

    private Vector3 originalPose;
    // Use this for initialization
    private static PhotonView ScenePhotonView;

    void Start()
    {
        cameraToolTransform.gameObject.SetActive(false);
        standbyCamera = GameObject.Find("StandbyCamera").GetComponent<Camera>();
        cameraTool = cameraToolTransform.GetComponent<Camera>();
        colliders = transform.GetComponents<Collider>();
        originalPose = transform.position;
    }



    // Detects if a tool is found by a user.
    void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            // When player collides with the camera, this message can be prompted to the screen
            // to remind user of a new tool.
            // TODO(sainan): Pop a sign on the top right corner.
            Debug.Log("Player found tool: " + transform.name + " !");
        }
    }

    // player tries to use the object.
    public void Use(Transform player)
    {
        user = player;
  
        SwitchViewPortToResizeCamera();
    }

    // Drop the camera.
    public void Done()
    {
        if (user == null)
        {
            return;
        }
        Debug.Log("Done using resizing tool!");
        SwitchViewPortToMainCamera();
        PutCameraDown();
        // Turn off collider to allow user to hold the camera.
        SetAllColliders(true);
        user = null;
    }

    // Set the target of the resize tool
    public void SetTarget(Transform target)
    {
        this.target = target;
    }

    public void Perform(float scale)
    {
        ResizeTarget(scale);
    }

    private void ResizeTarget(float scale)
    {
        if (target == null)
        {
            Debug.Log("Please select resize target first by clicking on the target!");
            return;
        }
        Debug.Log("ResizeTarget: " + target.name);
        target.GetComponent<PhotonView>().RPC("Resize", PhotonTargets.All, scale);
       
    }

    // Enable first person view, and make this tool start following the player.
    private void SwitchViewPortToResizeCamera()
    {
        // Enable first person view camera and tag as main to enable mouse interaction only in this view.
        cameraTool.gameObject.SetActive(true);
        cameraTool.tag = "MainCamera";
        // Set this camera below the overlay of screens.
        cameraTool.depth = 0;
        // Disable all colliders to avoid conflicts with the player.
        // TODO(sainan): we can also use layers to achieve this if we have time.
        SetAllColliders(false);

        // Disable the main camera to allow the new main camera to take effect.
        standbyCamera.gameObject.SetActive(false);
        // Port main camera as a small sub camera view on the top right corner.
        standbyCamera.rect = new Rect(-0.75f, 0.75f, 1.0f, 1.0f);
        // Layer this camera on top of the first person view.
        standbyCamera.depth = 1;
        // Give this camera a new tag.
        standbyCamera.tag = "DisabledMainCam";
        standbyCamera.gameObject.SetActive(true);
    }

    // Change active camera, and bring the main camera back.
    private void SwitchViewPortToMainCamera()
    {
        // Report main camera to full screen.
        standbyCamera.rect = new Rect(0.0f, 0.0f, 1.0f, 1.0f);
        standbyCamera.tag = "MainCamera";
        // Deactive the cameraTool.
        cameraTool.gameObject.SetActive(false);
        cameraTool.tag = "DisabledMainCam";
    }

    private void PutCameraDown()
    {
        originalPose.Set(user.position.x, originalPose.y, user.position.z);
        originalPose += user.forward * 10;
        transform.position = originalPose;
        // Doesn't matter which side the camera is facing.
    }

    // Enable or disable colliders of this object.
    private void SetAllColliders(bool enable)
    {
        foreach (Collider collider in colliders)
        {
            collider.enabled = enable;
        }
    }

    // Update to follow the player
    void LateUpdate()
    {
        if (user != null)
        {
            FollowPlayer();
        }
    }

    // Allow the resize tool to follow the player around while using.
    private void FollowPlayer()
    {
        if (user == null)
        {
            return;
        }

        // Calculate the current rotation angles
        //      float wantedRotationAngle = user.eulerAngles.y;
        float wantedHeight = user.position.y + height;

//        float currentRotationAngle = transform.eulerAngles.y;
        float currentHeight = transform.position.y;

//        // Damp the rotation around the y-axis
//        currentRotationAngle = Mathf.LerpAngle(currentRotationAngle, wantedRotationAngle, rotationDamping * Time.deltaTime);

        // Damp the height
        currentHeight = Mathf.Lerp(currentHeight, wantedHeight, heightDamping * Time.deltaTime);

//        // Convert the angle into a rotation
//        var currentRotation = Quaternion.Euler(0, currentRotationAngle, 0);

        // Set the position of the camera on the x-z plane to the position of the user:
        transform.position = user.position + user.forward * distance;

        // Set the height of the camera
        transform.position = new Vector3(transform.position.x, currentHeight, transform.position.z);

        // Always look at the target's head
        if (target != null)
        {
            transform.LookAt(target.position + user.up * 5.0f);
        }
        else
        {
            // If there is no target yet, look at a bit forward of the camera/user position.
            transform.LookAt(user.position + user.forward * distance * 2 + user.up * 5.0f);
        }
    }
}
