using Game.Scripts.Services.Input.Interfaces;
using UnityEngine;

namespace Game.Scripts.Services.Input
{
    public class InputService : IInputService
    {
        public Vector2 MoveInputAxis => InputReceiver.MoveInputAxis;
        public bool IsJumpPressed => InputReceiver.IsJumpPressed;
        public bool IsRunPressed => InputReceiver.IsRunPressed;
        public bool IsCrouchPressed => InputReceiver.IsCrouchPressed;
    }
} 