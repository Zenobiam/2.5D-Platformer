using Game.Scripts.Services.Ads.Interfaces;
using Game.Scripts.Services.PersistentProgress.Interfaces;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Scripts.UI.Windows.Shop
{
    /// <summary>
    /// Рекламный товар в окне магазина (урок 14): кнопка показа, объекты «реклама есть / нет», награда черепами.
    /// Зависимости передаёт ShopWindow.Construct (ручной DI — фабрика → окно → элемент), жизненный цикл —
    /// хуки ShopWindow (Initialize / Subscribe / Cleanup).
    /// </summary>
    public class RewardedAdItem : MonoBehaviour
    {
        public Button ShowAdButton;

        /// <summary>
        /// Видны, когда реклама готова (кнопка WATCH AD, иконка товара).
        /// </summary>
        public GameObject[] AdActiveObjects;

        /// <summary>
        /// Видны, когда рекламы нет («Ad not available»).
        /// </summary>
        public GameObject[] AdInactiveObjects;

        private IAdsService _adsService;
        private IPersistentProgressService _progressService;

        public void Construct(IAdsService adsService, IPersistentProgressService progressService)
        {
            _adsService = adsService;
            _progressService = progressService;
        }

        public void Initialize()
        {
            ShowAdButton.onClick.AddListener(OnShowAdClicked);
            RefreshAvailableAd();
        }

        public void Subscribe() =>
            _adsService.RewardedVideoReady += RefreshAvailableAd;

        public void Cleanup() =>
            _adsService.RewardedVideoReady -= RefreshAvailableAd;

        private void OnShowAdClicked() =>
            _adsService.ShowRewardedVideo(OnVideoFinished);

        private void OnVideoFinished() =>
            _progressService.Progress.WorldData.LootData.Add(_adsService.Reward);

        private void RefreshAvailableAd()
        {
            bool isVideoReady = _adsService.IsRewardedVideoReady;

            foreach (GameObject adActiveObject in AdActiveObjects)
                adActiveObject.SetActive(isVideoReady);

            foreach (GameObject adInactiveObject in AdInactiveObjects)
                adInactiveObject.SetActive(!isVideoReady);
        }
    }
}
