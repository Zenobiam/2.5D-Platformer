using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputController : MonoBehaviour
{
    private Vector2 moveInput;
    private bool isJumpPressed;
    private bool isRunPressed;
    private bool isCrouchPressed;

    public Vector2 MoveInput => moveInput;
    public bool IsJumpPressed => isJumpPressed;
    public bool IsRunPressed => isRunPressed;
    public bool IsCrouchPressed => isCrouchPressed;

    public void OnMove(InputAction.CallbackContext context)
    {
        moveInput = context.ReadValue<Vector2>();
    }

    public void OnJump(InputAction.CallbackContext context)
    {
        isJumpPressed = context.ReadValueAsButton();
    }

    public void OnRun(InputAction.CallbackContext context)
    {
        isRunPressed = context.ReadValueAsButton();
    }

    public void OnCrouch(InputAction.CallbackContext context)
    {
        isCrouchPressed = context.ReadValueAsButton();
    }
} 