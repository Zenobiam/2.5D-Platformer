using Game.Scripts.Data;
using TMPro;
using UnityEngine;

namespace Game.Scripts.UI.Elements
{
    /// <summary>
    /// Счётчик собранного лута на HUD. Данные — WorldData из прогресса, их передаёт GameFactory.CreateHud.
    /// </summary>
    public class LootCounter : MonoBehaviour
    {
        public TextMeshProUGUI Counter;
        private WorldData _worldData;

        public void Construct(WorldData worldData)
        {
            _worldData = worldData;
            _worldData.LootData.Changed += UpdateCounter;
        }

        private void Start() =>
            UpdateCounter();

        /// <summary>
        /// Прогресс живёт дольше HUD: без отписки после перезагрузки сцены событие получит уничтоженный счётчик.
        /// В курсе отписки нет (отступление, course-deviations.md).
        /// </summary>
        private void OnDestroy()
        {
            if (_worldData != null)
                _worldData.LootData.Changed -= UpdateCounter;
        }

        private void UpdateCounter() =>
            Counter.text = $"{_worldData.LootData.Collected}";
    }
}
