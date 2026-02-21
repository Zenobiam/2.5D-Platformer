using UnityEngine;

namespace Game.Scripts.Services.Input.Interfaces
{
    public interface IInputService
    {
        Vector2 MoveInputAxis { get; }
        bool IsJumpPressed { get; }
        bool IsRunPressed { get; }
        bool IsCrouchPressed { get; }
    }
}
