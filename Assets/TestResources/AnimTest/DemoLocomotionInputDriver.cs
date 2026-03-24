using NoWireAnim;
using UnityEngine;

[RequireComponent(typeof(AnimStateMachineRunner))]
public sealed class DemoLocomotionInputDriver : MonoBehaviour
{
    [SerializeField] private string speedKey = "Speed";
    [SerializeField] private string jumpingKey = "Jumping";

    private AnimStateMachineRunner _runner;

    private void Awake()
    {
        _runner = GetComponent<AnimStateMachineRunner>();
    }

    private void Update()
    {
        float moveX = Mathf.Abs(Input.GetAxisRaw("Horizontal"));
        _runner.SetFloat(speedKey, moveX > 0.1f ? 1f : 0f);

        bool isJumping = Input.GetKey(KeyCode.Space);
        _runner.SetFloat(jumpingKey, isJumping ? 1f : 0f);
    }
}