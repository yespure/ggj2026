using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Balls : ObjectController
{
    public override void OnPossessed()
    {
        base.OnPossessed();
        Debug.Log($"{name} possessed!");
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
            Debug.Log("∑≠ùL");
        }
    }
}
