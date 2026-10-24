using Game.Scripts.Core;
using Game.Scripts.Data;
using Game.Scripts.Data.Extensions;
using Game.Scripts.Infrastructure;
using Game.Scripts.Services.Input.Interfaces;
using Game.Scripts.Services.PersistentProgress;
using Game.Scripts.Services.PersistentProgress.Interfaces;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Game.Scripts.Player
{
    public class PlayerMovementController : MonoBehaviour, ISaveProgress
    {
        private PlayerFacing _facing;
        private PlayerAnimator _animator;
        private CharacterController _characterController;
        public float MovementSpeed;

        private IInputService _inputService;

        private void Awake()
        {
            _inputService = ServiceLocator.Container.Single<IInputService>();

            _facing = GetComponent<PlayerFacing>();
            _animator = GetComponentInChildren<PlayerAnimator>();
            _characterController = GetComponent<CharacterController>();
        }

        private void Update()
        {
            Vector3 _movementVector = Vector3.zero;

            _movementVector = UnityEngine.Camera.main.transform.TransformDirection(_inputService.MoveInputAxis);

            _movementVector.y = 0f;
            _movementVector = _movementVector.normalized;

            _movementVector += Physics.gravity;

            _characterController.Move(MovementSpeed * _movementVector * Time.deltaTime);

            _facing.Face(_movementVector);
            _animator.SetMoveDirection(_movementVector.x);
        }

        public void UpdateProgress(PlayerProgress progress)
        {
            progress.WorldData.PositionOnLevel =
                new PositionOnLevel(CurrentLevel(), transform.position.AsVector3Data());
        }

        public void LoadProgress(PlayerProgress progress)
        {
            if (CurrentLevel() == progress.WorldData.PositionOnLevel.Level)
            {
                var savedPosition = progress.WorldData.PositionOnLevel.Position;
                if (savedPosition != null)
                    Warp(to: savedPosition);
            }
        }

        private static string CurrentLevel() =>
            SceneManager.GetActiveScene().name;

        private void Warp(Vector3Data to)
        {
            _characterController.enabled = false;
            transform.position = to.AsVector3().AddY(_characterController.height);
            _characterController.enabled = true;
        }
    }
}