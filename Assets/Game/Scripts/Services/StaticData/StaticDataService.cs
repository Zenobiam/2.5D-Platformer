using System.Collections.Generic;
using System.Linq;
using Game.Scripts.Services.StaticData.Interfaces;
using Game.Scripts.StaticData;
using UnityEngine;

namespace Game.Scripts.Services.StaticData
{
    public class StaticDataService : IStaticDataService
    {
        private const string EnemiesDataPath = "StaticData/Enemies";
        private const string LevelsDataPath = "StaticData/Levels";

        private Dictionary<EnemyTypeId, EnemyStaticData> _enemies;
        private Dictionary<string, LevelStaticData> _levels;

        public void Load()
        {
            _enemies = Resources
                .LoadAll<EnemyStaticData>(EnemiesDataPath)
                .ToDictionary(x => x.EnemyTypeId, x => x);

            _levels = Resources
                .LoadAll<LevelStaticData>(LevelsDataPath)
                .ToDictionary(x => x.LevelKey, x => x);
        }

        public EnemyStaticData ForEnemy(EnemyTypeId typeId) => 
            _enemies.TryGetValue(typeId, out var staticData) 
                ? staticData 
                : null;

        public LevelStaticData ForLevel(string sceneKey) =>
            _levels.TryGetValue(sceneKey, out var staticData)
                ? staticData
                : null;
    }
}
