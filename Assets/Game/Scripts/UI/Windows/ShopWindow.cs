using TMPro;

namespace Game.Scripts.UI.Windows
{
    /// <summary>
    /// Окно магазина: число черепов (душ) из прогресса, обновляется при подборе лута.
    /// Отступление от курса: курс подписывает на LootData.Changed сам SubscribeUpdates (каждое изменение
    /// добавляет подписку, текст не обновляется) — здесь подписан RefreshSkullText (course-deviations.md).
    /// </summary>
    public class ShopWindow : WindowBase
    {
        public TextMeshProUGUI SkullText;

        protected override void Init() =>
            RefreshSkullText();

        protected override void SubscribeUpdates() =>
            Progress.WorldData.LootData.Changed += RefreshSkullText;

        protected override void Cleanup()
        {
            base.Cleanup();
            Progress.WorldData.LootData.Changed -= RefreshSkullText;
        }

        private void RefreshSkullText() =>
            SkullText.text = Progress.WorldData.LootData.Collected.ToString();
    }
}
