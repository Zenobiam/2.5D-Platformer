using Game.Scripts.BaseLogic.Interfaces;
using Game.Scripts.Enemy;
using UnityEngine;

namespace Game.Scripts.UI
{
    public class ActorUI : MonoBehaviour
    {
        public HpBar HpBar;
        private IHealth _health;

        public void Construct(IHealth health)
        {
            _health = health;
            _health.HealthChanged += UpdateHpBar;
        }

        // TEMP: старт для врагов, которых пока бросаем на сцену без фабрики.
        // Удалить в девятом уроке, когда враги будут создаваться через фабрику.
        private void Start()
        {
            IHealth health = GetComponent<IHealth>();

            if (health != null)
                Construct(health);
        }

        private void OnDestroy()
        {
            if (_health != null)
                _health.HealthChanged -= UpdateHpBar;
        }

        private void UpdateHpBar() =>
            HpBar.SetValue(_health.Current, _health.Max);
    }
}
