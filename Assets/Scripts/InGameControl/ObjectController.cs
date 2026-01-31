using Mirror;
using UnityEngine;
using UnityEngine.Events;

[System.Serializable]
public class ImpactEvent : UnityEvent<Vector3, Vector3> { }

public class ObjectController : NetworkBehaviour
{
    [Header("Stats")]
    public float moveSpeed = 5.0f;
    public float jumpForce = 5f;

    [Header("Impact Settings")]
    public float ejectThreshold = 10f;
    public MaskController currentPossessorMask;

    [Header("State")]
    [SyncVar] public bool isControlled = false;
    protected Rigidbody rb;
    protected bool isGrounded = true;

    protected float inputH;
    protected float inputV;
    protected bool jumpInput;

    [Header("VFX & SFX")]
    public ImpactEvent onHighImpact;

    protected virtual void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    protected virtual void Update()
    {
        if (!isControlled) return;
        if (!isOwned) return;

        inputH = Input.GetAxis("Horizontal");
        inputV = Input.GetAxis("Vertical");
        if (Input.GetKeyDown(KeyCode.Space)) jumpInput = true;

        Specialability();
    }

    protected virtual void FixedUpdate()
    {
        if (!isControlled) return;
        if (!isOwned) return;

        Move();
        Jump();
    }

    protected virtual void Move() { }

    protected virtual void Jump()
    {
        if (jumpInput && isGrounded)
        {
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            isGrounded = false;
        }
        jumpInput = false;
    }

    protected virtual void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = true;
            return;
        }

        // 1. 只有“我”（拥有控制权的本地玩家）才能准确检测撞击力度
        if (!isOwned) return;
        if (!isControlled || currentPossessorMask == null) return;

        float impactForce = collision.relativeVelocity.magnitude;

        // 如果力度足够大
        if (impactForce > ejectThreshold)
        {
            ContactPoint contact = collision.contacts[0];
            Vector3 hitPoint = contact.point;
            Vector3 hitNormal = contact.normal;

            // A. 计算我自己的反弹方向（远离碰撞点）
            Vector3 myEjectDir = collision.contacts[0].normal + Vector3.up * 0.5f;
            myEjectDir.Normalize();

            Debug.Log($"[Local] Impact {impactForce}, Ejecting Myself!");

            // 执行我自己的弹飞
            this.currentPossessorMask.ForceEject(myEjectDir * impactForce);

            // 告诉服务器：在这里播放特效
            CmdPlayImpactEffect(hitPoint, hitNormal);

            // B. 【新增】检查我撞到的是不是另一个玩家
            // 因为在对方的屏幕上，我可能只是个没有速度的幽灵，对方检测不到撞击
            ObjectController otherPlayer = collision.gameObject.GetComponent<ObjectController>();

            if (otherPlayer != null && otherPlayer.isControlled)
            {
                // 计算对方的被撞方向（和我相反）
                // 对方的反弹方向 = -(我的法线) = 朝着我撞击的方向被推出去
                Vector3 theirEjectDir = -collision.contacts[0].normal + Vector3.up * 0.5f;
                theirEjectDir.Normalize();

                Debug.Log($"[Network] Hit {otherPlayer.name}, Commanding them to Eject!");

                // 发送命令给服务器：由于我撞到了他，请让他也飞出去
                NetworkIdentity otherID = otherPlayer.GetComponent<NetworkIdentity>();
                if (otherID != null)
                {
                    CmdNotifyHit(otherID, theirEjectDir * impactForce);
                }
            }
        }
    }

    // --- 新增网络同步逻辑 ---

    // 1. 撞击者 -> 服务器：“我撞到了受害者，力度是 Force”
    [Command]
    void CmdNotifyHit(NetworkIdentity victimID, Vector3 force)
    {
        ObjectController victim = victimID.GetComponent<ObjectController>();

        // 服务器校验：确保受害者存在且被附身
        if (victim != null && victim.isControlled)
        {
            // 2. 服务器 -> 受害者：“你被撞了，按这个力度飞出去”
            // TargetRpc 只会发送给受害者的客户端
            victim.TargetEject(force);
        }
    }

    // 3. 受害者客户端接收指令
    [TargetRpc]
    public void TargetEject(Vector3 force)
    {
        // 再次检查本地状态，防止重复或错误弹出
        if (isControlled && currentPossessorMask != null)
        {
            Debug.Log($"[Server] Received Knockback: {force}");
            // 执行强制弹出
            currentPossessorMask.ForceEject(force);
        }
    }

    protected virtual void Specialability() { }

    public virtual void OnPossessed()
    {
        isControlled = true;
    }

    public virtual void OnUnPossessed()
    {
        isControlled = false;
        inputH = 0;
        inputV = 0;
    }

    [Command]
    void CmdPlayImpactEffect(Vector3 pos, Vector3 normal)
    {
        // 服务器广播给所有人（包括自己）
        RpcPlayImpactEffect(pos, normal);
    }

    // 【新增】特效执行
    [ClientRpc]
    void RpcPlayImpactEffect(Vector3 pos, Vector3 normal)
    {
        // 触发 UnityEvent
        // 所有客户端都会执行这里绑定在 Inspector 上的方法
        onHighImpact?.Invoke(pos, normal);
    }
}