using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character : ObjectController
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

}
