using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CommonPossessable : ObjectController
{
    protected override void Move()
    {
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");

        Vector3 camFwd = Camera.main.transform.forward;
        Vector3 camRight = Camera.main.transform.right;

        Vector3 moveDir = (camFwd * v + camRight * h).normalized;
        Vector3 moveVelocity = moveDir * moveSpeed;

        rb.AddForce(moveVelocity);
    }
}
