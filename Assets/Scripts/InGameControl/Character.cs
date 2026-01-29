using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character : ObjectController
{
    public static Action<bool> C_IsPossessing;    // been listened in RagDollController.cs
    //public Animator human_animator;

    void Start()
    {
        //human_animator = GetComponent<Animator>();      
    }

    public override void OnPossessed()
    {
        base.OnPossessed();
        Debug.Log($"{name} possessed!");

        C_IsPossessing?.Invoke(true);
    }

    public override void OnUnPossessed()
    {
        base.OnUnPossessed();
        Debug.Log($"{name} released!");

        C_IsPossessing?.Invoke(false);
    }

}
