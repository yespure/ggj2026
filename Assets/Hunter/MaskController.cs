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
    private NetworkTransformReliable netTrans; // Important

    private ObjectController currentPossessedObject;
    public bool IsPossessing => currentPossessedObject != null;

    void Awake()
    {
        // 在 Awake 获取组件，确保所有客户端（包括非LocalPlayer）都能初始化引用
        col = GetComponent<Collider>();
        rb = GetComponent<Rigidbody>();
        netTrans = GetComponent<NetworkTransformReliable>();
    }

    public override void OnStartLocalPlayer()
    {
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

    void HandleHoverMovement()
    {
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");

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

        if (rb.velocity.magnitude > 0.1f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(targetDirection);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * rotateSpeed);
        }
    }

    // --- 附身与解除逻辑 ---

    public void PossessTarget(ObjectController target)
    {
        if (!isLocalPlayer) return;
        if (target == null) return;

        NetworkIdentity targetNetID = target.GetComponent<NetworkIdentity>();
        if (targetNetID != null)
        {
            CmdPossess(targetNetID);
        }
    }

    public void UnPossessTarget()
    {
        if (!isLocalPlayer) return;
        if (currentPossessedObject == null) return;

        CmdUnPossess();
    }

    [Command]
    void CmdPossess(NetworkIdentity targetIdentity)
    {
        //try
        //{
        //    targetIdentity.AssignClientAuthority(connectionToClient);
        //}
        //catch (System.Exception e)
        //{
        //    Debug.LogWarning($"Assign Authority warning: {e.Message}");
        //}

        // 【关键】将物体的控制权（Authority）移交给请求附身的客户端
        targetIdentity.AssignClientAuthority(connectionToClient);
        RpcPossess(targetIdentity);
    }

    [ClientRpc]
    void RpcPossess(NetworkIdentity targetIdentity)
    {
        if (targetIdentity == null) return;
        ObjectController target = targetIdentity.GetComponent<ObjectController>();

        currentPossessedObject = target;

        // 1. 关闭面具物理
        if (rb)
        {
            rb.isKinematic = true;
            rb.velocity = Vector3.zero;
        }
        if (col) col.enabled = false;

        // 2. 【关键】关闭面具的网络同步，防止位置冲突
        if (netTrans) netTrans.enabled = false;

        // 3. 绑定父子级
        Transform maskSlot = target.transform.Find("MaskSlot");
        transform.SetParent(maskSlot != null ? maskSlot : target.transform);
        transform.localPosition = Vector3.zero;
        transform.localRotation = Quaternion.identity;
        transform.localScale = Vector3.one;

        // 4. 本地玩家操作
        if (isLocalPlayer)
        {
            SetupCamera(target.transform);
            target.OnPossessed(); // 开启输入
            Debug.Log($"[Player] Possessed: {target.name}");
        }
    }

    [Command]
    void CmdUnPossess()
    {
        // 在解除前获取 NetworkIdentity，因为解除后引用可能丢失
        NetworkIdentity targetIdentity = null;
        if (currentPossessedObject != null)
            targetIdentity = currentPossessedObject.GetComponent<NetworkIdentity>();

        RpcUnPossess();


        //if (targetIdentity != null)
        //{
        //    try
        //    {
        //        targetIdentity.RemoveClientAuthority();
        //    }
        //    catch (System.Exception e)
        //    {
        //        Debug.LogWarning($"Remove Authority warning: {e.Message}");
        //    }
        //}

        // 【关键】收回权限
        targetIdentity.RemoveClientAuthority();
    }

    [ClientRpc]
    void RpcUnPossess()
    {
        // 计算退出位置（向上弹起）
        Vector3 exitPos = transform.position;
        if (currentPossessedObject != null)
            exitPos = currentPossessedObject.transform.position + Vector3.up * 2f;

        // 本地玩家清理操作
        if (isLocalPlayer && currentPossessedObject != null)
        {
            currentPossessedObject.OnUnPossessed();
            SetupCamera(transform);
        }

        currentPossessedObject = null;

        // 1. 解除父子级
        transform.SetParent(null);
        transform.position = exitPos;
        transform.localScale = Vector3.one;

        // 2. 恢复物理
        if (rb) rb.isKinematic = false;
        if (col) col.enabled = true;

        // 3. 【关键】恢复网络同步
        if (netTrans) netTrans.enabled = true;

        Debug.Log("[Player] UnPossessed");
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