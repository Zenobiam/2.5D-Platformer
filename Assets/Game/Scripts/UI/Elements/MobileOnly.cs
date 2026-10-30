using UnityEngine;

namespace Game.Scripts.UI.Elements
{
    // Группа экранного управления HUD: видна только на мобильной платформе (и в Device Simulator).
    // UnityEngine.Device.Application, а не UnityEngine.Application: только его подменяет Device Simulator.
    public class MobileOnly : MonoBehaviour
    {
        private void Awake() =>
            gameObject.SetActive(UnityEngine.Device.Application.isMobilePlatform);
    }
}
