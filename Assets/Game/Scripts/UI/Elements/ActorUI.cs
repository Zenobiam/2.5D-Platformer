using Game.Scripts.BaseLogic.Interfaces;
using UnityEngine;

namespace Game.Scripts.UI.Elements
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

        // COURSE-TEMP: старт для врагов, брошенных на сцену без фабрики; курс оставляет его и после урока 13
        // (враги с урока 09 создаются фабрикой, которая сама зовёт Construct) — убрать, когда уберёт курс.
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
