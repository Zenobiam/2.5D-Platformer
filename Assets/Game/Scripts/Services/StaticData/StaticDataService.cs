using System.Collections.Generic;
using System.Linq;
using Game.Scripts.Services.StaticData.Interfaces;
using Game.Scripts.StaticData;
using UnityEngine;

namespace Game.Scripts.Services.StaticData
{
    public class StaticDataService : IStaticDataService
    {
        private Dictionary<EnemyTypeId, EnemyStaticData> _enemies;

        public void LoadEnemies()
        {
            _enemies = Resources
                .LoadAll<EnemyStaticData>($"StaticData/Enemies")
                .ToDictionary(x => x.EnemyTypeId, x => x);
        }

        public EnemyStaticData ForEnemy(EnemyTypeId typeId) => 
            _enemies.TryGetValue(typeId, out var staticData) 
                ? staticData 
                : null;
    }
}