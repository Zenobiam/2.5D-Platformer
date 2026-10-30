using Game.Scripts.Data;
using Game.Scripts.Services.PersistentProgress.Interfaces;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Scripts.UI.Windows
{
    /// <summary>
    /// База окна (урок 13): закрытие крестиком, доступ к прогрессу, хуки жизненного цикла.
    /// Новое окно — наследник: переопределяет Initialize (заполнить), SubscribeUpdates (подписаться), Cleanup (отписаться).
    /// Зависимость передаёт UIFactory через Construct до Start.
    /// </summary>
    public abstract class WindowBase : MonoBehaviour
    {
        /// <summary>
        /// Крестик окна. private + SerializeField: назначается в Inspector, снаружи не трогают. Не назначен — NRE в Awake.
        /// </summary>
        [SerializeField] private Button CloseButton;

        protected IPersistentProgressService ProgressService;
        protected PlayerProgress Progress => ProgressService.Progress;

        public void Construct(IPersistentProgressService progressService) =>
            ProgressService = progressService;

        private void Awake() =>
            OnAwake();

        private void Start()
        {
            Initialize();
            SubscribeUpdates();
        }

        private void OnDestroy() =>
            Cleanup();

        protected virtual void OnAwake() =>
            CloseButton.onClick.AddListener(() => Destroy(gameObject));

        protected virtual void Initialize() { }

        protected virtual void SubscribeUpdates() { }

        protected virtual void Cleanup() { }
    }
}
