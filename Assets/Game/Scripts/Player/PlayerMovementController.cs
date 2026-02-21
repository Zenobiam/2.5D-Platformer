using Game.Scripts.Core;
using Game.Scripts.Infrastructure;
using Game.Scripts.Services.Input.Interfaces;
using UnityEngine;

namespace Game.Scripts.Player
{
    public class PlayerMovementController : MonoBehaviour
    {
        private PlayerVisual _playerVisual;
        public CharacterController CharacterController;
        public float MovementSpeed;

        private IInputService _inputService;

        private void Awake()
        {   
            _playerVisual = GetComponentInChildren<PlayerVisual>();
            _inputService = ServiceLocator.Container.Single<IInputService>();
        }

        private void Update()
        {
            Vector3 _movementVector = Vector3.zero;

            _movementVector = Camera.main.transform.TransformDirection(_inputService.MoveInputAxis);

            _movementVector.y = 0f;
            _movementVector = _movementVector.normalized;

            _movementVector += Physics.gravity;

            CharacterController.Move(MovementSpeed * _movementVector * Time.deltaTime);

            _playerVisual.UpdateVisuals(_movementVector, CharacterController.isGrounded);
        }
    }
}