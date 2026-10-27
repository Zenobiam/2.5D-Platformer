using System;

namespace Game.Scripts.Data
{
    [Serializable]
    public class WorldData
    {
        public Vector3Data Position;
        public PositionOnLevel PositionOnLevel;
        public LootData LootData;

        public WorldData(string initialLevel)
        {
            PositionOnLevel = new PositionOnLevel(initialLevel);
            LootData = new LootData();
        }
    }
}