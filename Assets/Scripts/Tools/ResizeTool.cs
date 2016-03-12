using UnityEngine;
using System.Collections;

public class ResizeTool : MonoBehaviour, ToolInterface
{

    // The user is the one that controls the camera
    public Transform cameraToolTransform;
    public Transform mainCameraTransform;

    private Camera mainCamera;
    private Camera cameraTool;
    private Transform user;
    private Collider[] colliders;
    // The distance in the x-z plane to the target
    public float distance = 5.0f;
    // the height we want the camera to be above the target
    public float height = 5.0f;
    // How much we
    public float heightDamping = 2.0f;
    public float rotationDamping = 3.0f;

    private bool resizing;
    private Vector3 originalPose;
    // Use this for initialization
    void Start()
    {
        cameraToolTransform.gameObject.SetActive(false);
        mainCamera = mainCameraTransform.GetComponent<Camera>();
        cameraTool = cameraToolTransform.GetComponent<Camera>();
        colliders = transform.GetComponents<Collider>();
        originalPose = transform.position;
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            // When player collides with the camera, this message can be prompted to the screen
            // to remind user of a new tool.
            Debug.Log("Player found tool: " + transform.name + " !");
        }
    }
        
    // Update is called once per frame
    void Update()
    {

    }

    public void TryUse()
    {
        // If nobody is using the resizing tool, the user can use it.
        if (resizing)
        { 
            Debug.Log("Somebody else is using this tool.");
            return;
        }

        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        foreach (GameObject player in players)
        {
            // Only start to use the tool if the player was close to this tool right before, and it is the player's turn.
            if (player.GetPhotonView().isMine &&
                player.GetComponent<PlayerToolPack>().UseResizeTool().name == transform.name)
            {
                Debug.Log("Player " + player.name + " Now controls the resizing tool " + transform.name);
                user = player.transform;
                SwitchViewPortToResizeCamera();
                return;
            }
        }
        Debug.Log("Need to go closer to the tool.");

    }

    public void Done()
    {
        if (!resizing)
        {
            return;
        }
        Debug.Log("Done using resizing tool!");
        resizing = false;
        SwitchViewPortToMainCamera();
        PutCameraDown();
        user.GetComponent<PlayerToolPack>().ClearResizeTool();
        user = null;
        // Turn off collider to allow user to hold the camera.
        SetAllColliders(true);
    }

    private void SwitchViewPortToResizeCamera()
    {
        // Enable first person view, and attach to player.
        cameraTool.gameObject.SetActive(true);
        cameraTool.tag = "MainCamera";
        cameraTool.depth = 0;
        SetAllColliders(false);
        resizing = true;
        mainCamera.gameObject.SetActive(false);
        // Port main camera as a small sub camera view on the top right corner.
        // TODO: Adding second window here affects mouse control for turning.
        // because it is stil controlled through main camera casting.
        mainCamera.rect = new Rect(-0.75f, 0.75f, 1.0f, 1.0f);
        mainCamera.depth = 1;
        mainCamera.tag = "DisabledMainCam";
        mainCamera.gameObject.SetActive(true);
        // Change active camera, and bring the active camera back.
    }

    private void SwitchViewPortToMainCamera()
    {
        // Report main camera to full screen.
        mainCamera.rect = new Rect(0.0f, 0.0f, 1.0f, 1.0f);
        mainCamera.tag = "MainCamera";
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

    private void SetAllColliders(bool enable)
    {
        foreach (Collider collider in colliders)
        {
            collider.enabled = enable;
        }
    }

    void LateUpdate()
    {
        if (user != null && resizing)
        {
            FollowPlayer();
        }
    }

    private void FollowPlayer()
    {
        if (user == null)
        {
            return;
        }

        // Calculate the current rotation angles
        float wantedRotationAngle = user.eulerAngles.y;
        float wantedHeight = user.position.y + height;

        float currentRotationAngle = transform.eulerAngles.y;
        float currentHeight = transform.position.y;

        // Damp the rotation around the y-axis
        currentRotationAngle = Mathf.LerpAngle(currentRotationAngle, wantedRotationAngle, rotationDamping * Time.deltaTime);

        // Damp the height
        currentHeight = Mathf.Lerp(currentHeight, wantedHeight, heightDamping * Time.deltaTime);

        // Convert the angle into a rotation
        var currentRotation = Quaternion.Euler(0, currentRotationAngle, 0);

        // Set the position of the camera on the x-z plane to the position of the user:
        transform.position = user.position + user.forward * distance;

        // Set the height of the camera
        transform.position = new Vector3(transform.position.x, currentHeight, transform.position.z);

        // Always look at the target's head
        transform.LookAt(user.position + user.forward * distance * 2 + user.up * 5.0f);
    }
}
