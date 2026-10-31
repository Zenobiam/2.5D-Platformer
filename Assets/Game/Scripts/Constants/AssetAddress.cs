namespace Game.Scripts.Constants
{
    /// <summary>
    /// Адреса Addressables (урок 15, раньше — пути Resources в AssetPath). Адреса ставит Tools → Setup → Addressables:
    /// константы и AddressablesSetup меняются вместе, иначе загрузка падает только в рантайме (InvalidKeyException).
    /// </summary>
    public static class AssetAddress
    {
        public const string PlayerPath = "Player";
        public const string HudPath = "HUD";
        public const string Loot = "Loot";
        public const string Spawner = "SpawnPoint";
        public const string LevelTransferTrigger = "LevelTransferTrigger";
    }
}
