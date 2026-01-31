using UnityEngine;
using Mirror;

// 1. 必须改为继承 NetworkBehaviour
public class ObjectController : NetworkBehaviour
{
    [Header("Stats")]
    public float moveSpeed = 5.0f;
    public float jumpForce = 5f;

    [Header("State")]
    public bool isControlled = false;
    protected Rigidbody rb;
    protected bool isGrounded = true;

    protected float inputH;
    protected float inputV;
    protected bool jumpInput;

    protected virtual void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    protected virtual void Update()
    {
        if (!isControlled) return;

        // 2. 【关键】客机是否有控制权
        if (!isOwned) return;

        inputH = Input.GetAxis("Horizontal");
        inputV = Input.GetAxis("Vertical");
        if (Input.GetKeyDown(KeyCode.Space)) jumpInput = true;

        Specialability();
    }

    protected virtual void FixedUpdate()
    {
        if (!isControlled) return;

        // 3. 【关键】同上
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
}