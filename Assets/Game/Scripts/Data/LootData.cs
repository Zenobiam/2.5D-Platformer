using System;

namespace Game.Scripts.Data
{
    [Serializable]
    public class LootData
    {
        public int Collected;
        public LootPieceDataDictionary LootPiecesOnScene = new LootPieceDataDictionary();

        public Action Changed;

        public void Collect(Loot loot)
        {
            Collected += loot.Value;
            Changed?.Invoke();
        }

        /// <summary>
        /// Начислить черепа не из лута (награда за рекламу, урок 14).
        /// </summary>
        public void Add(int lootValue)
        {
            Collected += lootValue;
            Changed?.Invoke();
        }
    }
}