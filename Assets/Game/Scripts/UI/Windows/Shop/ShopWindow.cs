using Game.Scripts.Services.Ads.Interfaces;
using Game.Scripts.Services.PersistentProgress.Interfaces;
using TMPro;

namespace Game.Scripts.UI.Windows.Shop
{
    /// <summary>
    /// Окно магазина: число черепов (душ) из прогресса и рекламный товар (урок 14).
    /// Свой Construct с IAdsService — цена ручного DI: UIFactory знает конкретный тип окна и его зависимости.
    /// Отступление от курса: курс подписывает на LootData.Changed сам SubscribeUpdates (урок 13) — здесь RefreshSkullText.
    /// </summary>
    public class ShopWindow : WindowBase
    {
        public TextMeshProUGUI SkullText;
        public RewardedAdItem AdItem;

        public void Construct(IAdsService adsService, IPersistentProgressService progressService)
        {
            base.Construct(progressService);
            AdItem.Construct(adsService, progressService);
        }

        protected override void Initialize()
        {
            AdItem.Initialize();
            RefreshSkullText();
        }

        protected override void SubscribeUpdates()
        {
            AdItem.Subscribe();
            Progress.WorldData.LootData.Changed += RefreshSkullText;
        }

        protected override void Cleanup()
        {
            base.Cleanup();
            AdItem.Cleanup();
            Progress.WorldData.LootData.Changed -= RefreshSkullText;
        }

        private void RefreshSkullText() =>
            SkullText.text = Progress.WorldData.LootData.Collected.ToString();
    }
}
