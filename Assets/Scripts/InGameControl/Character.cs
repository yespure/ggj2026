using UnityEngine;

public class Character : ObjectController
{
    private void Start()
    {
        
    }

    protected override void Move()
    {
        if (!isOwned) return;

        // 输入检测
        if (Mathf.Abs(inputH) < 0.01f && Mathf.Abs(inputV) < 0.01f)
        {
            // 如果没有输入，应该让速度衰减（可选）
            // rb.velocity = Vector3.Lerp(rb.velocity, Vector3.zero, Time.deltaTime * 5f);

            return;
        }

        // 1. 获取相机方向并【抹平 Y 轴】
        Vector3 camFwd = Camera.main.transform.forward;
        Vector3 camRight = Camera.main.transform.right;

        camFwd.y = 0;
        camRight.y = 0;

        // 2. 重新归一化 (因为抹平Y轴后，向量长度变短了)
        camFwd.Normalize();
        camRight.Normalize();

        // 3. 计算目标方向
        Vector3 targetDirection = (camFwd * inputV + camRight * inputH).normalized;

        // 4. 计算速度
        Vector3 targetVelocity = targetDirection * moveSpeed;

        // 5. 应用移动 (保留你的 Lerp 手感)
        // 注意：保留原本的 Y 轴速度（重力），否则角色会掉不下去或浮空
        Vector3 finalVelocity = Vector3.Lerp(rb.velocity, targetVelocity, Time.deltaTime * 5f);
        finalVelocity.y = rb.velocity.y; // 关键：不要覆盖掉物理引擎计算的重力/跳跃速度
        rb.velocity = finalVelocity;

        // 6. 转向逻辑
        // 只有当有显著移动意图时才转向，防止原地抖动
        if (targetDirection.magnitude > 0.1f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(targetDirection);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 10);
        }
    }

    public override void OnPossessed()
    {
        base.OnPossessed();
        Debug.Log($"{name} possessed! Authority/Ownership: {isOwned}");
    }
}