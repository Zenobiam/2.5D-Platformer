using System;

namespace Game.Scripts.Data
{
    /// <summary>
    /// Неподобранный лут на сцене: где лежит и сколько стоит.
    /// </summary>
    [Serializable]
    public class LootPieceData
    {
        public Vector3Data Position;
        public Loot Loot;

        public LootPieceData(Vector3Data position, Loot loot)
        {
            Position = position;
            Loot = loot;
        }
    }
}
