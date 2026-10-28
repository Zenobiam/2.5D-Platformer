using System;
using UnityEngine;

namespace Game.Scripts.StaticData
{
    /// <summary>
    /// Спавнер уровня: id (из UniqueId маркера — по нему прогресс помнит убитых), тип врага и позиция.
    /// </summary>
    [Serializable]
    public class EnemySpawnerStaticData
    {
        public string Id;
        public EnemyTypeId EnemyTypeId;
        public Vector3 Position;

        public EnemySpawnerStaticData(string id, EnemyTypeId enemyTypeId, Vector3 position)
        {
            Id = id;
            EnemyTypeId = enemyTypeId;
            Position = position;
        }
    }
}
