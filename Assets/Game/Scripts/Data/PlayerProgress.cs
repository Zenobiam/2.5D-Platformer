using System;

namespace Game.Scripts.Data
{
    [Serializable]
    public class PlayerProgress
    {
        public WorldData WorldData;
        
        public PlayerProgress(string initialLevel)
        {
           WorldData = new WorldData(initialLevel);
        }
    }
}