using System;
using Game.Scripts.Animation.Interfaces;
using UnityEngine;

namespace Game.Scripts.Enemy
{
    public class EnemyAnimator : MonoBehaviour, IAnimationStateReader
    {
        private static readonly int SpeedHash = Animator.StringToHash("Speed");
        private static readonly int DeathHash = Animator.StringToHash("Death");
        private static readonly int DamageHash = Animator.StringToHash("Damage");
        private static readonly int SplashAttackHash = Animator.StringToHash("SplashAttack");
        private static readonly int LeftAttackHash = Animator.StringToHash("LeftAttack");
        private static readonly int RightAttackHash = Animator.StringToHash("RightAttack");
        private static readonly int LocomotionHash = Animator.StringToHash("Locomotion");

        private Animator _animator;
        
        public event Action<AnimatorState> StateEntered;
        public event Action<AnimatorState> StateExited;

        public AnimatorState State { get; private set; }

        private void Awake()
        {
            _animator = GetComponent<Animator>();
        }

        // Пустой OnAnimatorMove: аниматор не двигает корень — это делает NavMeshAgent.
        private void OnAnimatorMove()
        {
        }

        public void Move(float speed) =>
            _animator.SetFloat(SpeedHash, Mathf.Clamp01(speed));

        public void StopMoving() =>
            _animator.SetFloat(SpeedHash, 0);

        public void PlayLeftAttack() => _animator.SetTrigger(LeftAttackHash);

        public void PlayRightAttack() => _animator.SetTrigger(RightAttackHash);

        public void PlaySplashAttack() => _animator.SetTrigger(SplashAttackHash);

        public void PlayDamage() => _animator.SetTrigger(DamageHash);

        public void PlayDeath() => _animator.SetTrigger(DeathHash);

        public void EnteredState(int stateHash)
        {
            State = StateFor(stateHash);
            StateEntered?.Invoke(State);
        }

        public void ExitedState(int stateHash) => 
            StateExited?.Invoke(StateFor(stateHash));

        private static AnimatorState StateFor(int stateHash)
        {
            if (stateHash == LocomotionHash)
                return AnimatorState.Locomotion;
            if (stateHash == LeftAttackHash)
                return AnimatorState.LeftAttack;
            if (stateHash == RightAttackHash)
                return AnimatorState.RightAttack;
            if (stateHash == SplashAttackHash)
                return AnimatorState.SplashAttack;
            if (stateHash == DamageHash)
                return AnimatorState.Damage;
            if (stateHash == DeathHash)
                return AnimatorState.Death;

            return AnimatorState.Unknown;
        }
    }
}
