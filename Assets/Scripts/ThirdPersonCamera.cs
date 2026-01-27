using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThirdPersonCamera : MonoBehaviour
{
    [Header("Ò•½Ç¸úëS")]
    public Transform target;
    public float followSpeed = 10f;
    public float rotateSpeed = 10f;
    public Vector3 offset = new Vector3(0f, 1.6f, -4f);

    [Header("”zÏñ™CÒÆ„Ó")]
    public float mouseSensitivity = 120f;
    public float minPitch = -30f;
    public float maxPitch = 60f;
    private float yaw;
    private float pitch;

    void LateUpdate()
    {
        CameraFollow();
       // CameraMove();
    }

    private void CameraMove()
    {
        //”zÏñ™C¹ ‡úÄÚÒÆ„Ó
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

        yaw += mouseX;
        pitch -= mouseY;
        pitch = Mathf.Clamp(pitch, minPitch, maxPitch);


        transform.rotation = Quaternion.Euler(pitch, yaw, 0f);
    }
    private void CameraFollow()
    {
        //”zÏñ™CµÚÈýÈË·Q¸úëS
        if (!target) return;
        Vector3 desiredPosition = target.position + target.rotation * offset;
        transform.position = Vector3.Lerp(transform.position, desiredPosition, followSpeed * Time.deltaTime);
        Vector3 lookTarget = target.position + Vector3.up * 1.5f;
        Quaternion targetRotation = Quaternion.LookRotation(lookTarget - transform.position);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotateSpeed * Time.deltaTime);
    }
}
    
