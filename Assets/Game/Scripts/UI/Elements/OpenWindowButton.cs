using Game.Scripts.UI.Services.Windows;
using Game.Scripts.UI.Services.Windows.Interfaces;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Scripts.UI.Elements
{
    /// <summary>
    /// Кнопка, открывающая окно по WindowId (урок 13). Сервис передаёт GameFactory.CreateHud через Init —
    /// кнопка, созданная не фабрикой, при нажатии упадёт с NRE.
    /// </summary>
    public class OpenWindowButton : MonoBehaviour
    {
        public Button Button;
        public WindowId WindowId;

        private IWindowService _windowService;

        public void Init(IWindowService windowService) =>
            _windowService = windowService;

        private void Awake() =>
            Button.onClick.AddListener(Open);

        private void Open() =>
            _windowService.Open(WindowId);
    }
}
