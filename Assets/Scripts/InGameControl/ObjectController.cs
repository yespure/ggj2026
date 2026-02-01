using Mirror;
using UnityEngine;
using UnityEngine.Events;

[System.Serializable]
public class ImpactEvent : UnityEvent<Vector3, Vector3> { }

// 1. 必须改为继承 NetworkBehaviour
public class ObjectController : NetworkBehaviour
{
    [Header("Stats")]
    public float moveSpeed = 5.0f;
    public float jumpForce = 5f;
    public float impactFactor = 0.5f;

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

    // 【新增】缓存上一帧的速度，用于碰撞时的“拼刀”判定
    // 设为 public 是为了让对方能读取到我的碰撞前速度
    [HideInInspector] public Vector3 previousVelocity;

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
        // 【关键改动】
        // 在所有逻辑之前，先记录这一帧开始时的速度（也就是碰撞前的速度）
        // 无论是不是 isOwned，只要挂了脚本都需要记录，这样别人撞我的时候，能查到我原本的速度
        if (rb != null)
        {
            previousVelocity = rb.velocity;
        }

        if (!isControlled) return;
        if (!isOwned) return;

        Move();
        Jump();
    }

    protected virtual void Move()
    {
        // 基类留空
    }

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

        if (!isOwned) return;
        if (!isControlled || currentPossessorMask == null) return;

        float impactForce = collision.relativeVelocity.magnitude;

        if (impactForce > ejectThreshold)
        {
            ContactPoint contact = collision.contacts[0];
            Vector3 hitPoint = contact.point;
            Vector3 hitNormal = contact.normal;

            ObjectController otherPlayer = collision.gameObject.GetComponent<ObjectController>();

            // 默认反弹方向
            Vector3 myEjectDir = hitNormal + Vector3.up * 0.5f;
            myEjectDir.Normalize();

            if (otherPlayer != null && otherPlayer.isControlled)
            {
                // === PvP 逻辑：速度对决 ===

                // 【核心修复】使用 previousVelocity (碰撞前速度) 进行对比
                float mySpeed = this.previousVelocity.magnitude;
                float theirSpeed = otherPlayer.previousVelocity.magnitude;

                float winMargin = 2.0f;

                Debug.Log($"[PvP Check] My Pre-Vel: {mySpeed:F1} | Their Pre-Vel: {theirSpeed:F1}");

                if (mySpeed > theirSpeed + winMargin)
                {
                    // 【我赢了】
                    Debug.Log($"[PvP] I Won! I stay, they fly.");

                    CmdPlayImpactEffect(hitPoint, hitNormal);

                    Vector3 theirEjectDir = -hitNormal + Vector3.up * 0.5f;
                    theirEjectDir.Normalize();

                    NetworkIdentity otherID = otherPlayer.GetComponent<NetworkIdentity>();
                    if (otherID != null)
                    {
                        CmdNotifyHit(otherID, theirEjectDir * impactForce * impactFactor);
                    }
                }
                else if (theirSpeed > mySpeed + winMargin)
                {
                    // 【我输了】
                    // 等待对方发指令让我飞，或者双重保险稍微弹一点
                    Debug.Log($"[PvP] I Lost. Waiting for command.");
                }
                else
                {
                    // 【平局】
                    Debug.Log($"[PvP] Draw! Both Eject.");
                    this.currentPossessorMask.ForceEject(myEjectDir * impactForce * impactFactor);
                    CmdPlayImpactEffect(hitPoint, hitNormal);
                }
            }
            else
            {
                // === PvE 逻辑 ===
                this.currentPossessorMask.ForceEject(myEjectDir * impactForce * impactFactor);
                CmdPlayImpactEffect(hitPoint, hitNormal);
            }
        }
    }

    [Command]
    void CmdNotifyHit(NetworkIdentity victimID, Vector3 force)
    {
        ObjectController victim = victimID.GetComponent<ObjectController>();
        if (victim != null && victim.isControlled)
        {
            victim.TargetEject(force);
        }
    }

    [TargetRpc]
    public void TargetEject(Vector3 force)
    {
        if (isControlled && currentPossessorMask != null)
        {
            Debug.Log($"[Server] Received Knockback: {force}");
            currentPossessorMask.ForceEject(force);
        }
    }

    [Command]
    void CmdPlayImpactEffect(Vector3 pos, Vector3 normal)
    {
        RpcPlayImpactEffect(pos, normal);
    }

    [ClientRpc]
    void RpcPlayImpactEffect(Vector3 pos, Vector3 normal)
    {
        onHighImpact?.Invoke(pos, normal);
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
}