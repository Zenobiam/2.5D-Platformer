using System.Collections.Generic;
using UnityEngine;

namespace Game.Scripts.StaticData
{
    /// <summary>
    /// Статические данные уровня: ключ — имя сцены, спавнеры собирает кнопка Collect (LevelStaticDataEditor)
    /// из SpawnMarker на сцене; позицию игрока и перехода (урок 15) — по тегам InitialPoint и LevelTransferInitialPoint.
    /// </summary>
    [CreateAssetMenu(fileName = "LevelData", menuName = "StaticData/Level")]
    public class LevelStaticData : ScriptableObject
    {
        public string LevelKey;
        public List<EnemySpawnerStaticData> EnemySpawners;
        public Vector3 InitialPlayerPosition;
        public LevelTransferStaticData LevelTransfer;
    }
}
