using UnityEngine;
using Mirror;
using Cinemachine;

public class FakePlayer : NetworkBehaviour
{
    [Header("Hover Settings")]
    public float moveSpeed = 10f;
    public float rotateSpeed = 10f; // 模型转身的速度
    public float deceleration = 5f; // 停止时的惯性阻尼

    [Header("References")]
    private CinemachineFreeLook freeLookCam;
    private Transform mainCamTransform;
    private Vector3 currentVelocity;

    public override void OnStartLocalPlayer()
    {
        freeLookCam = FindObjectOfType<CinemachineFreeLook>();
        mainCamTransform = Camera.main.transform;

        if (freeLookCam != null)
        {
            freeLookCam.Follow = transform;
            freeLookCam.LookAt = transform;

            // 确保相机处于 World Space 模式，否则相机视角会受玩家旋转影响
            freeLookCam.m_BindingMode = CinemachineTransposer.BindingMode.WorldSpace;
        }

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
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

        // 如果没有输入，就进行减速（阻尼效果），并不改变朝向，此时玩家可以自由转动相机，而不影响角色
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

        transform.position += currentVelocity * Time.deltaTime;

        // 旋转角色模型
        if (currentVelocity.magnitude > 0.1f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(targetDirection);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * rotateSpeed);
        }
    }
}