using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectController : MonoBehaviour
{
    [Header("移動")]
    public float moveSpeed = 1.0f;
    public float turnSpeed = 40f;

    [Header("跳跃")]
    public float jumpForce = 5f;
    public bool isGrounded = true;
    protected  Rigidbody rb;

    [Header("Controljudgement")]
    public bool isControlled = false;
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        
    }

    // Update is called once per frame
    void Update()
    {
        if (!isControlled) return;
        Move();
        Jump();
        Specialability();
    }
    //以下本地坐標移動
    protected virtual void Move()
    {

        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        {
            //轉向邏輯
            Quaternion targetRotation = Quaternion.Euler(
                0f,
                horizontal * turnSpeed * Time.fixedDeltaTime,
                0f
                );
            rb.MoveRotation(rb.rotation * targetRotation);
            //移動邏輯
            Vector3 move = transform.forward * vertical * moveSpeed * Time.fixedDeltaTime;
            rb.MovePosition(rb.position + move);
        }
    }
    //以下世界坐標移動
    /*
    private void Move()
    {
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        Vector3 inputDir = new Vector3(horizontal, 0f, vertical);

        if (inputDir.magnitude > 0.1f)
        {
            // 转向
            Quaternion targetRotation = Quaternion.LookRotation(inputDir);
            transform.rotation = Quaternion.Slerp(
                transform.rotation,
                targetRotation,
                Time.deltaTime * turnSpeed
            );

            // 用输入方向移动（关键！）
            rb.MovePosition(
                rb.position + inputDir.normalized * moveSpeed * Time.deltaTime
            );
        }
    }
    */

    protected virtual void Jump()
    {
        //跳躍邏輯(Rigidbody)
        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            isGrounded = false;
        }
    }

    protected virtual void OnCollisionEnter(Collision collision)
    {
        //地面檢測
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = true;
        }
    }

    protected virtual void Specialability()
    {

    }
    public virtual void OnPossessed()
    {
        isControlled = true;
    }

    public virtual void OnUnPossessed()
    {
        isControlled = false;
    }

}
