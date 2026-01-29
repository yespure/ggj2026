using System.Collections;
using System.Collections.Generic;
using Mirror.Examples.Pong;
using UnityEngine;


// This script should be hang on all possessed objects.
public class RagDollController : MonoBehaviour
{
    // Human has Animator, when set it to enabled = false, it will turn human to ragdoll state.
    // Attention: ragdoll state is only for human obj.

    //Components
    private Character character;
    private Ball ball;

    public Rigidbody rb;
    public Collider colli;
    public Transform ts;

    public bool C_mode;
    public bool B_mode;

    // Parameters
    

    /// <summary>
    /// public static Action<bool> IsPossessing;   // Add this into the script manage the possessing, assume it is FakePlayer.cs
    /// 
    /// IsPossessing?.Invoke(some boolean);
    /// </summary>

    void Start()
    {
        character = GetComponent<Character>();

        rb = GetComponent<Rigidbody>();
        colli = GetComponent<Collider>();
        ts = GetComponent<Transform>();
    }
    
    void OnEnable()
    {
        Character.C_IsPossessing += C_IsDead;
        Balls.B_IsPossessing += B_IsDead;
    }

    void OnDisable()
    {
        Character.C_IsPossessing -= C_IsDead;
        Balls.B_IsPossessing -= B_IsDead;
    }

    void Update()
    {
        C_IsDead(C_mode);
        B_IsDead(B_mode);
    }

    void C_IsDead(bool isPossessed)
    {
        //if (character.human_animator == null & rb == null) return;

        //character.human_animator.enabled = !isPossessed;
    }

    void B_IsDead(bool isPossessed)
    {
        if (rb == null) return;

        rb.useGravity = isPossessed;
        rb.freezeRotation = !isPossessed;

    }


}
