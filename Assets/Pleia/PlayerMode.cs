using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMode : MonoBehaviour
{
    // Hang this script on Player with other scripts.
    // This script only manages the functions of different modes, not the condition.
    // Conditions should be managed in other scripts, like FakePlayer.cs

    BoardPainter boardPainter;      // let boardPainter = true when enter the lobby, false when leave the lobby

    //Components
    public Rigidbody m_rb;
    public Collider m_colli;
    public Transform m_transform;

    // Parameters
    bool m_drawing;
    bool[] playerState = new bool[2]; // 0: drawing; 1: ragdoll; 2: normal

    /// <summary>
    /// 
    /// public static Action<bool> RagdollOn;   // Add this into the script manage the conditions, assume it is FakePlayer.cs
    /// public static Action<bool> OnDrawing;
    /// public static Action<bool> IsNormal;
    /// 
    /// 
    /// RagdollOn?.Invoke(some boolean);
    /// OnDrawing?.Invoke(some boolean);
    /// IsNormal?.Invoke(some boolean);
    /// </summary>

    void OnEnable()
    {
        // FakePlayer.RagdollOn += MaskRagDoll;
        // FakePlayer.OnDrawing += DrawingMode;
        // FakePlayer.IsNormal += NormalMode;
    }
    void ODisable()
    {
        // FakePlayer.RagdollOn -= MaskRagDoll;
        // FakePlayer.OnDrawing -= DrawingMode;
        // FakePlayer.IsNormal -= NormalMode;
    }


    public void Start()
    {
        boardPainter.enabled = true;
        m_rb = GetComponent<Rigidbody>();
        m_colli = GetComponent<Collider>();
        m_transform = GetComponent<Transform>();
    }


    void Update()
    {
        // if (Input.GetKeyDown(KeyCode.H))
        // {
        //     m_ragdollOn = true;
        // }
        DrawingMode(playerState[0]);
        MaskRagDoll(playerState[1]);
        NormalMode(!playerState[2]);
    }

    public void MaskRagDoll(bool ragdollOn)
    {
        if (m_rb == null) return;

        // This is for Mask, not for special obj.
        // When ragdollOn is true, enable ragdoll:
        m_rb.useGravity = !ragdollOn;
        m_rb.freezeRotation = ragdollOn;

    }

    public void DrawingMode(bool canDraw)
    {
        if (m_rb == null) return;

        boardPainter.enabled = canDraw;
    }

    public void NormalMode(bool isNormal)
    {
        if (m_rb == null) return;

        m_rb.useGravity = !isNormal; // No gravity when normal.
        m_rb.freezeRotation = isNormal;
        m_colli.enabled = isNormal;
    }
}
