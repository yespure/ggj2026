using System.Collections;
using System.Collections.Generic;
using UnityEngine;


// This script is for special objects with ragdoll physics.
public class RagDollController : MonoBehaviour
{
    // Assume special obj has Animator, when set it to enabled = false, it will turn human to ragdoll state.
    // Attention: ragdoll state is only for human obj, controlled by animator.

    //Components
    private Animator human_animator;
    public Rigidbody rb;
    public Collider colli;
    public Transform ts;

    // Parameters
    

    /// <summary>
    /// public static Action<bool> IsPossessing;   // Add this into the script manage the possessing, assume it is FakePlayer.cs
    /// 
    /// IsPossessing?.Invoke(some boolean);
    /// </summary>

    void Start()
    {
        human_animator = GetComponent<Animator>();

        rb = GetComponent<Rigidbody>();
        colli = GetComponent<Collider>();
        ts = GetComponent<Transform>();
    }
    
    void OnEnable()
    {
        // FakePlayer.IsPossessing += IsDead;
        
    }

    void OnDisable()
    {
        // FakePlayer.IsPossessing -= IsDead;
    }

    void Update()
    {
        
    }

    void IsDead(bool isPossessed)
    {
        if (this.human_animator == null)
        {
            // Set other's rigidbody and collider
        }
        else {human_animator.enabled = !isPossessed;}
    

    }


}
