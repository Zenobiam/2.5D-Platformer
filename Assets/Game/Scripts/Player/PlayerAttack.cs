using Game.Scripts.BaseLogic.Interfaces;
using Game.Scripts.Data;
using Game.Scripts.Enemy;
using Game.Scripts.Infrastructure;
using Game.Scripts.Services.Input.Interfaces;
using Game.Scripts.Services.PersistentProgress.Interfaces;
using UnityEngine;

namespace Game.Scripts.Player
{
    // Ставится на объект с Animator (CyborgGirl): событие клипа Hit вызывает OnAttack на этом же объекте.
    [RequireComponent(typeof(PlayerAnimator))]
    public class PlayerAttack : MonoBehaviour , ISaveProgressReader
    {
        private LayerMask _hittableMask;
        private readonly Collider[] _hits = new Collider[3];

        private IInputService _input;
        public PlayerAnimator Animator;
        public CharacterController CharacterController;
        private Stats _playerStats;
        private PlayerFacing _facing;


        private void Awake()
        {
            _input = ServiceLocator.Container.Single<IInputService>();
            // PlayerFacing на корне Player, а этот компонент — на модели (CyborgGirl).
            _facing = GetComponentInParent<PlayerFacing>();

            _hittableMask = 1 << LayerMask.NameToLayer("Hittable");
        }

        private void Update()
        {
            if (_input.AttackButtonUp() && !Animator.IsAttacking)
                Animator.PlayAttack();
        }

        // Вызывается событием клипа Hit (0:05).
        public void OnAttack()
        {
            PhysicsDebug.DrawDebug(StartPoint() + _facing.Forward, _playerStats.DamageRadius, 1f);

            for (int i = 0; i < Hit(); i++)
            {
                _hits[i].transform.parent.GetComponent<IHealth>().TakeDamage(_playerStats.Damage);
            }
        }

        public void LoadProgress(PlayerProgress progress)
        {
            _playerStats = progress.PlayerStats;
        }

        private int Hit() =>
            Physics.OverlapSphereNonAlloc(StartPoint() + _facing.Forward, _playerStats.DamageRadius, _hits, _hittableMask);

        // В курсе y = center.y / 2 (пол на y = 0); у нас уровень выше, поэтому от позиции игрока.
        private Vector3 StartPoint() =>
            new Vector3(transform.position.x, transform.position.y + CharacterController.center.y / 2, transform.position.z);
    }
}
