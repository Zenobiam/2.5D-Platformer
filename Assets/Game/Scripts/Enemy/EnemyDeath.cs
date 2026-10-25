using System;
using System.Collections;
using Game.Scripts.Player;
using UnityEngine;
using UnityEngine.AI;

namespace Game.Scripts.Enemy
{
    [RequireComponent(typeof(EnemyHealth), typeof(EnemyAnimator))]
    public class EnemyDeath : MonoBehaviour
    {
        public EnemyHealth Health;
        public EnemyAnimator  Animator;

        public GameObject DeathFx;
        
        public event Action DeathAction;

        private EnemyAttack _attack;
        private Follow[] _follows;
        private NavMeshAgent _agent;
        private AnimateAlongAgent _animateAlongAgent;

        private void Awake()
        {
            _attack = GetComponent<EnemyAttack>();
            _follows = GetComponents<Follow>();
            _agent = GetComponent<NavMeshAgent>();
            _animateAlongAgent = GetComponent<AnimateAlongAgent>();
        }

        private void Start() =>
            Health.HealthChanged += OnHealthChanged;

        private void OnDestroy() => 
            Health.HealthChanged -= OnHealthChanged;

        private void OnHealthChanged()
        {
            if (Health.Current <= 0)
                Die();
        }

        private void Die()
        {
            Health.HealthChanged -= OnHealthChanged;
            StopBehaviour();
            Animator.PlayDeath();
            
            SpawnDeathFx();
            
            StartCoroutine(DestroyCorpseTimer());

            DeathAction?.Invoke();
        }

        // Атаки Skeleton идут из Any State, поэтому труп нужно остановить, иначе он уходит из Death.
        // Коллайдеры выключаем все: AggroZone и AttackZone (триггеры Aggro/атаки) и HitBox (удары по трупу).
        private void StopBehaviour()
        {
            if (_attack != null)
                _attack.enabled = false;

            foreach (Follow follow in _follows)
                follow.enabled = false;

            // Агент перезаписывает позицию корня каждый кадр и ломает падение из клипа Death.
            // AnimateAlongAgent читает выключенного агента с ошибкой, поэтому выключаем его первым.
            if (_animateAlongAgent != null)
                _animateAlongAgent.enabled = false;

            if (_agent != null)
                _agent.enabled = false;

            foreach (Collider collider in GetComponentsInChildren<Collider>())
                collider.enabled = false;
        }

        private void SpawnDeathFx() =>
            Instantiate(DeathFx, transform.position, Quaternion.identity);

        private IEnumerator DestroyCorpseTimer()
        {
            yield return new WaitForSeconds(3f); // TODO вынести таймер
            Destroy(gameObject);
        }
    }
}