using UnityEngine;
using UnityEngine.InputSystem;

namespace Game.Scripts.Services.Input
{
    /// <summary>
    /// MonoBehaviour, который получает callbacks от PlayerInput в сцене.
    /// Вешать на тот же GameObject, что и PlayerInput.
    /// </summary>
    public class InputReceiver : MonoBehaviour
    {
        public static Vector2 MoveInputAxis => _moveInput;
        public static bool IsJumpPressed => _isJumpPressed;
        public static bool IsRunPressed => _isRunPressed;
        public static bool IsCrouchPressed => _isCrouchPressed;

        private static Vector2 _moveInput;
        private static bool _isJumpPressed;
        private static bool _isRunPressed;
        private static bool _isCrouchPressed;

        public void OnMove(InputAction.CallbackContext context)
        {
            _moveInput = context.ReadValue<Vector2>();
        }

        public void OnJump(InputAction.CallbackContext context)
        {
            _isJumpPressed = context.ReadValueAsButton();
        }

        public void OnRun(InputAction.CallbackContext context)
        {
            _isRunPressed = context.ReadValueAsButton();
        }

        public void OnCrouch(InputAction.CallbackContext context)
        {
            _isCrouchPressed = context.ReadValueAsButton();
        }
    }
}
