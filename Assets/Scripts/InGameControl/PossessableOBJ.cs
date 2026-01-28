using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PossessableOBJ : MonoBehaviour
{
    public void OnPossessed()
    {
        Debug.Log($"{gameObject.name} 被附身了！");
        // 之后你可以在这里切换控制权
    }
}
