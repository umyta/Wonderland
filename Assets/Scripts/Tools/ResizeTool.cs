using UnityEngine;
using System.Collections;

[RequireComponent(typeof(PhotonView))]
[RequireComponent(typeof(FollowPlayer))]
[RequireComponent(typeof(Rigidbody))]
public class ResizeTool : MonoBehaviour, ToolInterface
{
    // The user is the one that controls the camera
    public Transform cameraToolTransform;
    public Transform maxResizeTransform;
    public Transform minResizeTransform;
    private Camera standbyCamera;
    private Camera cameraTool;
    private FollowPlayer toolFollow;
    private Transform user;
    private Transform target;
    private Collider[] colliders;
    private ToolStatus status;

    private Vector3 originalPose;
    // Use this for initialization
    private static PhotonView ScenePhotonView;

    // A percentage of the original size.
    private float scalingFactor = 0.0f;
    private Rect SMALL_SUB_WINDOW_SIZE = new Rect(-0.75f, 0.75f, 1.0f, 1.0f);
    private Rect FULL_SCREEN_WINDOW_SIZE = new Rect(0.0f, 0.0f, 1.0f, 1.0f);
    private float maxSize = 0.0f;
    private float minSize = 0.0f;
    // Updated whenever a target is set.
    private float targetOriginalSize = 0.0f;

    void Start()
    {
        cameraToolTransform.gameObject.SetActive(false);
        standbyCamera = GameObject.Find("StandbyCamera").GetComponent<Camera>();
        cameraTool = cameraToolTransform.GetComponent<Camera>();
        colliders = transform.GetComponents<Collider>();
        originalPose = transform.position;
        toolFollow = transform.GetComponent<FollowPlayer>();
        maxSize = maxResizeTransform.GetComponent<BoxCollider>().size.x;
        minSize = minResizeTransform.GetComponent<BoxCollider>().size.x;
        Debug.Log("Min size " + minSize + " Max size " + maxSize);
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
        targetOriginalSize = 0.0f;
    }

    // Set the target of the resize tool
    public void SetTarget(Transform target)
    {
        this.target = target;
        targetOriginalSize = target.GetComponent<BoxCollider>().size.x;
        Debug.Log("Target size: " + targetOriginalSize);
    }

    public void TryPerform(float scale)
    {
        if (user == null)
        {
            Debug.Log("Can't perform resizing, please click on the target first");
        }
        ResizeTarget(scale); 
    }

    public ToolStatus GetStatus()
    {
        status.factor = (scalingFactor * targetOriginalSize / targetOriginalSize) * 100;
        return status;
    }

    // Expect scale to be a number between 0 to 1.
    private void AdjustedScaleFactor(float scale)
    {
        float minScaleFactor = minSize / targetOriginalSize;
        float maxScaleFactor = maxSize / targetOriginalSize;
        Debug.Log("min scaling factor is " + minScaleFactor);
        Debug.Log("max scaling factor is " + maxScaleFactor);
        Debug.Log("scale is " + scale);
        scalingFactor = minScaleFactor + scale * (maxScaleFactor - minScaleFactor);
    }

    private void ResizeTarget(float scale)
    {
        if (target == null)
        {
            Debug.LogWarning("Please select resize target first by clicking on the target!");
            return;
        }
        AdjustedScaleFactor(scale); // Set scaling factor.
        Debug.Log("Trying to resizing " + targetOriginalSize + " to " + scalingFactor * targetOriginalSize);
        target.GetComponent<PhotonView>().RPC("Resize", PhotonTargets.All, scalingFactor);       
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
        standbyCamera.rect = SMALL_SUB_WINDOW_SIZE;
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
        standbyCamera.rect = FULL_SCREEN_WINDOW_SIZE;
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
        if (user != null && toolFollow != null)
        {
            toolFollow.PlayerFollowTarget(user, target);
        }
    }

}
