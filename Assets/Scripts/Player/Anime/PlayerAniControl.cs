using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class PlayerAniControl : MonoBehaviour
{
    [SerializeField] private Animator animator;
    [SerializeField] private MovementController movement;
    [SerializeField] private PlayerController player;

    [Header("animator parameters")]
    public string pSpeed = "Speed";
    public string pVSpeed = "VSpeed";         // float
    public string pGrounded = "Grounded";     // bool
    public string pCrouching = "Crouching";   // bool (可空)
    public string pJumping = "Jumping";       // bool (可空)
    public string pDashing = "Dashing";       // bool
    public string pClimbing = "Climbing";     // bool (可空)
    public string pAiming = "Aiming";         // bool (可空)

    [Header("Animator Triggers")]
    public string tJump = "Jump";             // trigger (可空)
    public string tLand = "Land";             // trigger (可空)
    public string tDash = "Dash";             // trigger (可空)
    public string tThrow = "Throw";           // trigger (可空)

    private bool prevGrounded;
    private bool prevDashing;
    private bool prevJumping;
    private bool prevAiming;

    private void Awake()
    {
        if (!animator) animator = GetComponent<Animator>();
        if (!movement) movement = GetComponent<MovementController>();
        if (!player) player = GetComponent<PlayerController>();

        prevGrounded = movement != null && movement.IsGrounded;
        prevDashing = movement != null && movement.IsDashing;
        prevJumping = movement != null && movement.IsJumping;
        prevAiming = player != null && player.IsAiming;
    }

    private void Update()
    {
        if (!animator || !movement) return;

        float speed = Mathf.Abs(movement.HorizongtalVelocity);
        float vSpeed = movement.VerticalVelocity;

        bool grounded = movement.IsGrounded;
        bool dashing = movement.IsDashing;
        bool crouching = movement.IsCrouching;
        bool jumping = movement.IsJumping;

        bool climbing = player != null && player.IsClimbing;
        bool aiming = player != null && player.IsAiming;

        // ---- 写参数（持续态）----
        SetFloat(pSpeed, speed);
        SetFloat(pVSpeed, vSpeed);

        SetBool(pGrounded, grounded);
        SetBool(pDashing, dashing);

        SetBool(pCrouching, crouching);
        SetBool(pJumping, jumping);
        SetBool(pClimbing, climbing);
        SetBool(pAiming, aiming);

        // ---- 触发器（瞬间态：用边沿变化触发一次）----
        if (!prevJumping && jumping) Trigger(tJump);
        if (!prevGrounded && grounded) Trigger(tLand);
        if (!prevDashing && dashing) Trigger(tDash);
        if (prevAiming && !aiming) Trigger(tThrow);

        prevGrounded = grounded;
        prevDashing = dashing;
        prevJumping = jumping;
        prevAiming = aiming;
    }

    private void SetFloat(string name, float v)
    {
        if (string.IsNullOrEmpty(name)) return;
        animator.SetFloat(name, v);
    }

    private void SetBool(string name, bool v)
    {
        if (string.IsNullOrEmpty(name)) return;
        animator.SetBool(name, v);
    }

    private void Trigger(string name)
    {
        if (string.IsNullOrEmpty(name)) return;
        animator.SetTrigger(name);
    }
}