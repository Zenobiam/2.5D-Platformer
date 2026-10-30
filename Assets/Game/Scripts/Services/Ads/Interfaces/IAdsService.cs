using System;
using Game.Scripts.Infrastructure;

namespace Game.Scripts.Services.Ads.Interfaces
{
    /// <summary>
    /// Реклама за вознаграждение (урок 14), интерфейс как в курсе. Окно не знает, какой SDK за ним:
    /// сейчас StubAdsService (ПК), позже — LevelPlay для мобильной сборки.
    /// </summary>
    public interface IAdsService : IService
    {
        event Action RewardedVideoReady;
        bool IsRewardedVideoReady { get; }
        int Reward { get; }
        void Initialize();
        void ShowRewardedVideo(Action onVideoFinished);
    }
}
