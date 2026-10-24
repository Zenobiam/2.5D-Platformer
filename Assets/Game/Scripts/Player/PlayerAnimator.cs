using UnityEngine;

namespace Game.Scripts.Player
{
    [RequireComponent(typeof(Animator))]
    public class PlayerAnimator : MonoBehaviour
    {
        private static readonly int SpeedHash = Animator.StringToHash("Speed");
        // Триггер Damage в FreeHandController (состояния Damage1-3)
        private static readonly int DamageHash = Animator.StringToHash("Damage");
        private static readonly int DeathHash = Animator.StringToHash("Death");
        // Триггер Attack и состояние Hit (клип удара) — должны быть связаны в FreeHandController
        private static readonly int AttackHash = Animator.StringToHash("Attack");
        private static readonly int HitStateHash = Animator.StringToHash("Hit");

        private Animator _animator;

        public bool IsAttacking => _animator.GetCurrentAnimatorStateInfo(0).shortNameHash == HitStateHash;

        private void Awake() =>
            _animator = GetComponent<Animator>();

        // CC двигает root — не даём root motion клипам сдвигать CharacterController.
        private void OnAnimatorMove()
        {
        }

        // Yurowm FreeHand: Speed 0 idle / 1 run
        public void SetMoveDirection(float moveX)
        {
            float speedParam = Mathf.Abs(moveX) > 0.1f ? 1f : 0f;
            _animator.SetFloat(SpeedHash, speedParam);
        }

        public void PlayDamage() =>
            _animator.SetTrigger(DamageHash);

        // Сбрасываем отложенный Attack: иначе Any State → Hit уводит из Death сразу после смерти.
        public void PlayDeath()
        {
            _animator.ResetTrigger(AttackHash);
            _animator.SetTrigger(DeathHash);
        }

        public void PlayAttack() =>
            _animator.SetTrigger(AttackHash);
    }
}
