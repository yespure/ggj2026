using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Balls : ObjectController
{
    public static Action<bool> B_IsPossessing;    // been listened in RagDollController.cs

    public override void OnPossessed()
    {
        base.OnPossessed();
        Debug.Log($"{name} possessed!");

        B_IsPossessing?.Invoke(true);
    }

    public override void OnUnPossessed()
    {
        base.OnUnPossessed();
        Debug.Log($"{name} released!");
    }
    protected override void Specialability()
    {
        base.Specialability();
        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            Debug.Log("���L");
        }
    }
}
