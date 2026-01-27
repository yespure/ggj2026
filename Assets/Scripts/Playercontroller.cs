using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Playercontroller : MonoBehaviour
{
    [Header("移動")]
    private float moveSpeed = 1.0f;
    private float turnSpeed = 40f;

    [Header("跳跃")]
    public float jumpForce = 5f;
    public bool isGrounded = true;
    private Rigidbody rb;
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        Move();
        Jump();
    }
    //以下本地坐標移動
    private void Move()
    {
        
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");
        
        {
            //轉向邏輯
            Quaternion targetRotation = Quaternion.Euler (
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

    private void Jump()
    {
        //跳躍邏輯(Rigidbody)
        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            isGrounded = false;
        }
    }
    
    private void OnCollisionEnter(Collision collision)
    {
        //地面檢測
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = true;
        }
    }
}
