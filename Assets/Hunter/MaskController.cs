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

    // 【修改点1】组件引用必须在所有端都存在
    private Rigidbody rb;
    private Collider col;
    private NetworkTransformReliable netTrans;

    private ObjectController currentPossessedObject;

    public bool IsPossessing => currentPossessedObject != null;

    // 【修改点2】使用 Awake 初始化组件，确保所有客户端（包括非控制者）都有这些引用
    void Awake()
    {
        col = GetComponent<Collider>();
        rb = GetComponent<Rigidbody>();
        netTrans = GetComponent<NetworkTransformReliable>();
    }

    // 【修改点3】OnStartLocalPlayer 只处理“只有本地玩家需要做的事”（如相机、输入）
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
        RpcPossess(targetIdentity);
    }

    [ClientRpc]
    void RpcPossess(NetworkIdentity targetIdentity)
    {
        if (targetIdentity == null) return;
        ObjectController target = targetIdentity.GetComponent<ObjectController>();

        currentPossessedObject = target;

        // 【安全检查】虽然放在Awake里了，多一层检查防止极端情况
        if (rb)
        {
            rb.isKinematic = true;
            rb.velocity = Vector3.zero;
        }
        if (col) col.enabled = false;

        // 【关键】禁用网络同步
        if (netTrans) netTrans.enabled = false;

        Transform maskSlot = target.transform.Find("MaskSlot");
        transform.SetParent(maskSlot != null ? maskSlot : target.transform);
        transform.localPosition = Vector3.zero;
        transform.localRotation = Quaternion.identity;
        transform.localScale = Vector3.one;

        if (isLocalPlayer)
        {
            SetupCamera(target.transform);
            target.OnPossessed();
            Debug.Log($"[Player] Possessed: {target.name}");
        }
    }

    [Command]
    void CmdUnPossess()
    {
        RpcUnPossess();
    }

    [ClientRpc]
    void RpcUnPossess()
    {
        Vector3 exitPos = transform.position;
        if (currentPossessedObject != null) exitPos = currentPossessedObject.transform.position + Vector3.up * 1.5f;

        if (isLocalPlayer && currentPossessedObject != null)
        {
            currentPossessedObject.OnUnPossessed();
            SetupCamera(transform);
        }

        currentPossessedObject = null;

        transform.SetParent(null);
        transform.position = exitPos;
        transform.localScale = Vector3.one;

        if (rb) rb.isKinematic = false;
        if (col) col.enabled = true;

        // 【关键】重新启用网络同步
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