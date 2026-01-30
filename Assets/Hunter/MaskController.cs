using UnityEngine;
using Mirror;
using Cinemachine;

public class MaskController : NetworkBehaviour
{
    [Header("Hover Settings")]
    public float moveSpeed = 10f;
    public float rotateSpeed = 10f;
    public float deceleration = 5f;

    // Refs
    private CinemachineFreeLook freeLookCam;
    private Transform mainCamTransform;
    private Rigidbody rb;
    private Collider col;

    private ObjectController currentPossessedObject;

    public bool IsPossessing => currentPossessedObject != null;

    public override void OnStartLocalPlayer()
    {
        col = GetComponent<Collider>();
        rb = GetComponent<Rigidbody>();
        mainCamTransform = Camera.main.transform;

        freeLookCam = FindObjectOfType<CinemachineFreeLook>();
        if (freeLookCam != null)
        {
            SetupCamera(transform);
            freeLookCam.m_BindingMode = CinemachineTransposer.BindingMode.WorldSpace;
        }
    }

    void FixedUpdate()
    {
        if (!isLocalPlayer) return;
        if (IsPossessing) return;

        HandleHoverMovement();
    }

    /// <summary>
    /// Core movement logic when hovering
    /// </summary>
    void HandleHoverMovement()
    {
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");

        // Decelerate to stop if no input
        if (Mathf.Abs(h) < 0.01f && Mathf.Abs(v) < 0.01f)
        {
            rb.velocity = Vector3.Lerp(rb.velocity, Vector3.zero, Time.deltaTime * deceleration);
            return;
        }

        Vector3 camFwd = mainCamTransform.forward;
        Vector3 camRight = mainCamTransform.right;

        Vector3 targetDirection = (camFwd * v + camRight * h).normalized;
        Vector3 targetVelocity = targetDirection * moveSpeed;

        rb.velocity = Vector3.Lerp(rb.velocity, targetVelocity, Time.deltaTime * 5f);

        // Rotate towards movement direction
        if (rb.velocity.magnitude > 0.1f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(targetDirection);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * rotateSpeed);
        }
    }

    // --- CORE MECHANICS ---

    /// <summary>
    /// Possession logic
    /// 1. attaching to target object
    /// 2. switching camera
    /// 3. notifying target object
    /// </summary>
    public void PossessTarget(ObjectController target)
    {
        if (target == null) return;

        currentPossessedObject = target;

        // Physical state changes
        rb.isKinematic = true;
        rb.velocity = Vector3.zero;
        col.enabled = false;

        // Visual attachment
        Transform maskSlot = target.transform.Find("MaskSlot");
        transform.SetParent(maskSlot != null ? maskSlot : target.transform);
        transform.localPosition = Vector3.zero;
        transform.localRotation = Quaternion.identity;
        transform.localScale = Vector3.one;

        // Camera switch
        SetupCamera(target.transform);

        // Notify target
        target.OnPossessed();

        Debug.Log($"[Player] Possessed: {target.name}");
    }

    /// <summary>
    /// Unpossession logic
    /// 1. notifying target object
    /// 2. restoring physical state
    /// 3. switching camera back
    /// </summary>
    public void UnPossessTarget()
    {
        if (currentPossessedObject == null) return;

        // notify target object
        currentPossessedObject.OnUnPossessed();
        currentPossessedObject = null;

        // restore physical state and detach
        rb.isKinematic = false;
        col.enabled = true;
        transform.SetParent(null);
        transform.localScale = Vector3.one;

        // switch camera back
        SetupCamera(transform);

        // transform.position += Vector3.up * 1.5f;

        Debug.Log("[FakePlayer] UnPossessed");
    }

    private void SetupCamera(Transform target)
    {
        if (freeLookCam != null)
        {
            freeLookCam.Follow = target;
            freeLookCam.LookAt = target;
        }
    }
}