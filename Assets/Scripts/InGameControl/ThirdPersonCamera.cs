using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThirdPersonCamera : MonoBehaviour
{
    [Header("×·×ÙÉèÖÃ")]
    public Transform target;
    public float followSpeed = 10f;
    public Vector3 offset = new Vector3(0f, 1.6f, -4f);

    [Header("Êó±ê¿ØÖÆ")]
    public float mouseSensitivity = 200f;
    public float minPitch = -20f;
    public float maxPitch = 45f;

    private float yaw;
    private float pitch;

    void Start()
    {
        yaw = 0f;
        pitch = 0f;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        CameraMove();
    }

    void LateUpdate()
    {
        CameraFollow();
    }

    void CameraMove()
    {
        yaw += Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        pitch -= Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;
        pitch = Mathf.Clamp(pitch, minPitch, maxPitch);
    }

    void CameraFollow()
    {
        if (!target) return;
        Quaternion rotation = Quaternion.Euler(pitch, target.eulerAngles.y + yaw, 0);
        Vector3 desiredPosition = target.position + rotation * offset;
        transform.position = Vector3.Lerp(transform.position, desiredPosition, followSpeed * Time.deltaTime);
        Vector3 lookPoint = target.position + Vector3.up * offset.y;
        transform.LookAt(lookPoint);
    }
}
