using System;
using Game.Scripts.Services.Ads.Interfaces;

namespace Game.Scripts.Services.Ads
{
    /// <summary>
    /// Заглушка рекламы для ПК (решение 2026-10-30, course-deviations.md). Курс делает AdsService на Unity Ads
    /// Advertisement Legacy — с 31.01.2026 Unity не поддерживает прямую интеграцию через этот пакет (замена — LevelPlay),
    /// и реклама Unity работает только на Android/iOS. Заглушка всегда «готова» и сразу засчитывает полный просмотр —
    /// так проверяется вся цепочка урока (кнопка → сервис → награда) без SDK. Реальный сервис — отдельный класс
    /// с тем же интерфейсом, регистрация в BootstrapState.RegisterAdsService.
    /// </summary>
    public class StubAdsService : IAdsService
    {
        private const int RewardAmount = 15;

        public event Action RewardedVideoReady;

        public bool IsRewardedVideoReady { get; private set; }

        public int Reward => RewardAmount;

        public void Initialize()
        {
            IsRewardedVideoReady = true;
            RewardedVideoReady?.Invoke();
        }

        public void ShowRewardedVideo(Action onVideoFinished)
        {
            if (!IsRewardedVideoReady)
                return;

            onVideoFinished?.Invoke();
        }
    }
}
