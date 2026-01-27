using Mirror;
using UnityEngine;

public class FakePlayer : NetworkBehaviour
{
    [Header("Settings")]
    public float moveSpeed = 5f;
    public float sensitivity = 2f;

    [Header("References")]
    public Camera playerCamera;

    private float pitchAngleX = 0f;
    private float yawAngleY = 0f;

    // Mirror specific method
    // When this object is set up as the local player, this method is automatically called
    public override void OnStartLocalPlayer()
    {
        if (playerCamera != null)
        {
            playerCamera.gameObject.SetActive(true);
        }

        //Cursor.lockState = CursorLockMode.Locked;
        //Cursor.visible = false;
    }

    // When the object loaded, ensure only the local player's camera is active
    void Start()
    {
        if (!isLocalPlayer && playerCamera != null)
        {
            playerCamera.gameObject.SetActive(false);
            AudioListener listener = playerCamera.GetComponent<AudioListener>();
            if (listener) listener.enabled = false;
        }
    }

    void Update()
    {
        // **IMPORTANT**: Only process input for the local player
        if (!isLocalPlayer) return;

        HandleRotation();
        HandleMovement();
    }

    void HandleRotation()
    {
        float mouseXInput = Input.GetAxis("Mouse X") * sensitivity;
        float mouseYInput = Input.GetAxis("Mouse Y") * sensitivity;

        yawAngleY += mouseXInput;
        pitchAngleX -= mouseYInput;

        pitchAngleX = Mathf.Clamp(pitchAngleX, -89f, 89f);

        Quaternion targetRotation = Quaternion.Euler(pitchAngleX, yawAngleY, 0f);
        transform.localRotation = targetRotation;
    }

    void HandleMovement()
    {
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");

        Vector3 direction = (transform.forward * v + transform.right * h).normalized;

        transform.Translate(direction * moveSpeed * Time.deltaTime, Space.World);
    }
}
