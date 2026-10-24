using System;

namespace Game.Scripts.BaseLogic.Interfaces
{
    public interface IHealth
    {
        event Action HealthChanged;
        float Current { get; set; }
        float Max { get; set; }
        void TakeDamage(float damage);
    }
}