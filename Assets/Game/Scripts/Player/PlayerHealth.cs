using System;
using Game.Scripts.BaseLogic.Interfaces;
using Game.Scripts.Data;
using Game.Scripts.Enemy;
using Game.Scripts.Services.PersistentProgress.Interfaces;
using UnityEngine;

namespace Game.Scripts.Player
{
    //[RequireComponent(typeof(PlayerAnimator))]
    public class PlayerHealth : MonoBehaviour, ISaveProgress, IHealth
    {
        public PlayerAnimator PlayerAnimator;
        private State _state;

        public event Action HealthChanged;

        public float Current
        {
            get => _state.CurrentHp;
            set
            {
                if (_state.CurrentHp != value)
                {
                    _state.CurrentHp = value;
                    HealthChanged?.Invoke();
                }
            }
        }

        public float Max
        {
            get => _state.MaxHp;
            set => _state.MaxHp = value;
        }

        public void LoadProgress(PlayerProgress progress)
        {
            _state = progress.PlayerState;
            HealthChanged?.Invoke();
        }

        public void UpdateProgress(PlayerProgress progress)
        {
            progress.PlayerState.CurrentHp = Current;
            progress.PlayerState.MaxHp = Max;
        }

        public void TakeDamage(float damage)
        {
            if (Current <= 0)
                return;
            Current -= damage;
            PlayerAnimator.PlayDamage();
        }
    }
}