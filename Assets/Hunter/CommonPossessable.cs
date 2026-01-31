using UnityEngine;
using Mirror;

public class CommonPossessable : ObjectController
{
    protected override void Move()
    {
        // 【关键】检查 isOwned
        if (!isOwned) return;

        if (Mathf.Abs(inputH) < 0.01f && Mathf.Abs(inputV) < 0.01f) return;

        Vector3 camFwd = Camera.main.transform.forward;
        Vector3 camRight = Camera.main.transform.right;

        camFwd.y = 0;
        camRight.y = 0;
        camFwd.Normalize();
        camRight.Normalize();

        Vector3 moveDir = (camFwd * inputV + camRight * inputH).normalized;
        Vector3 moveVelocity = moveDir * moveSpeed;

        rb.AddForce(moveVelocity);
    }

    // 可选：如果需要在附身时有额外逻辑
    public override void OnPossessed()
    {
        base.OnPossessed();
        Debug.Log($"{name} possessed! Authority/Ownership: {isOwned}");
    }
}