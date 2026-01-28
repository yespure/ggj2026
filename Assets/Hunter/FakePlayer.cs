using UnityEngine;
using Mirror;
using Cinemachine;
using System;

public class FakePlayer : NetworkBehaviour
{
    [Header("Hover Settings")]
    public float moveSpeed = 10f;
    public float rotateSpeed = 10f;
    public float deceleration = 5f; // stop damping

    [Header("References")]
    private CinemachineFreeLook freeLookCam;
    private Transform mainCamTransform;
    private Vector3 currentVelocity;

    public override void OnStartLocalPlayer()
    {
        freeLookCam = FindObjectOfType<CinemachineFreeLook>();
        mainCamTransform = Camera.main.transform;

        if (freeLookCam == null) Debug.LogError("No Cinemachine FreeLook Camera found in the scene.");
        if (freeLookCam != null)
        {
            freeLookCam.Follow = transform;
            freeLookCam.LookAt = transform;

            // Ensure the camera maintains its world space orientation
            freeLookCam.m_BindingMode = CinemachineTransposer.BindingMode.WorldSpace;
        }

        //Cursor.lockState = CursorLockMode.Locked;
        //Cursor.visible = false;
    }

    void Update()
    {
        if (!isLocalPlayer) return;

        HandleMovement();
    }

    void HandleMovement()
    {
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");

        // If no input, decelerate to a stop
        if (Mathf.Abs(h) < 0.01f && Mathf.Abs(v) < 0.01f)
        {
            currentVelocity = Vector3.Lerp(currentVelocity, Vector3.zero, Time.deltaTime * deceleration);
            transform.position += currentVelocity * Time.deltaTime;
            return;
        }

        Vector3 camFwd = mainCamTransform.forward;
        Vector3 camRight = mainCamTransform.right;

        Vector3 targetDirection = (camFwd * v + camRight * h).normalized;
        Vector3 targetVelocity = targetDirection * moveSpeed;

        currentVelocity = Vector3.Lerp(currentVelocity, targetVelocity, Time.deltaTime * 5f);
        Debug.Log(currentVelocity);

        transform.position += currentVelocity * Time.deltaTime;

        // Rotate towards movement direction
        if (currentVelocity.magnitude > 0.1f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(targetDirection);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * rotateSpeed);
        }
    }
}