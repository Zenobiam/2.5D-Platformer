using UnityEngine;

namespace Game.Scripts.Player
{
    [RequireComponent(typeof(PlayerHealth))]
    public class PlayerDeath : MonoBehaviour
    {
        public PlayerHealth PlHealth;

        public PlayerMovementController PlMovementController;
        public PlayerAnimator PlAnimator;
        public PlayerAttack PlAttack;

        public GameObject DeathFx;
        private bool _isDead;

        private void Start() =>
            PlHealth.HealthChanged += OnPlHealthChanged;

        private void OnDestroy() =>
            PlHealth.HealthChanged -= OnPlHealthChanged;

        private void OnPlHealthChanged()
        {
            if (!_isDead && PlHealth.Current <= 0)
                Die();
        }

        private void Die()
        {
            _isDead = true;

            PlMovementController.enabled = false;
            PlAttack.enabled = false;

            if (TryGetComponent(out Rigidbody body))
                body.isKinematic = true;
            PlAnimator.PlayDeath();

            Instantiate(DeathFx, transform.position, Quaternion.identity);
        }
    }
}