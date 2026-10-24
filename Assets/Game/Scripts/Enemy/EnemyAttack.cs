using System.Linq;
using Game.Scripts.BaseLogic.Interfaces;
using Game.Scripts.Factories.Interfaces;
using Game.Scripts.Infrastructure;
using Game.Scripts.Player;
using UnityEngine;

namespace Game.Scripts.Enemy
{
    [RequireComponent(typeof(EnemyAnimator))]
    public class EnemyAttack : MonoBehaviour
    {
        public EnemyAnimator Animator;

        public float AttackCooldown = 3f;
        public float Cleavage = 0.5f;
        public float EffecticveDistance = 0.5f;
        public float Damage = 10f;

        private IGameFactory _gameFactory;
        private Transform _playerTransform;
        private float _attackCooldown;
        private bool _isAttacking;
        private int _layerMask;
        private Collider[] _hits = new Collider[1];
        private bool _attackIsActive;

        private void Awake()
        {
            _gameFactory = ServiceLocator.Container.Single<IGameFactory>();
            
            _layerMask = 1 << LayerMask.NameToLayer("Player");
            _gameFactory.PlayerCreated += OnPlayerCreated;
        }

        private void Update()
        {
            UpdateCooldown();

            if (CanAttack())
                StartAttack();
        }

        private void OnAttack()
        {
            if (Hit(out Collider hit))
            {
                PhysicsDebug.DrawDebug(StartPoint(), Cleavage, 1);
                hit.transform.GetComponent<IHealth>().TakeDamage(Damage);
            }
        }

        private bool Hit(out Collider hit)
        {
            int hitsCount = Physics.OverlapSphereNonAlloc(StartPoint(), Cleavage, _hits, _layerMask);

            hit = _hits.FirstOrDefault();
            
            return hitsCount > 0;
        }

        private Vector3 StartPoint() => 
            new Vector3(transform.position.x, transform.position.y + 0.5f, transform.position.z) + transform.forward*EffecticveDistance;


        private void OnAttackEnded()
        {
            _attackCooldown = AttackCooldown;
            _isAttacking = false;
        }

        private void UpdateCooldown()
        {
            if (!CoolDownIsUp())
                _attackCooldown -= Time.deltaTime;
        }

        private void StartAttack()
        {
            transform.LookAt(_playerTransform);
            Animator.PlayRightAttack();

            _isAttacking = true; 
        }

        private bool CanAttack() => 
            _attackIsActive && !_isAttacking && CoolDownIsUp();

        private bool CoolDownIsUp() => 
            _attackCooldown <= 0;

        private void OnPlayerCreated() => 
            _playerTransform = _gameFactory.PlayerGameObject.transform;

        public void EnableAttack() => 
            _attackIsActive = true;

        public void DisableAttack() => 
            _attackIsActive = false;
    }
}