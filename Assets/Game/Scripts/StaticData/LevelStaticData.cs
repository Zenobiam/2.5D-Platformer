using System.Collections.Generic;
using UnityEngine;

namespace Game.Scripts.StaticData
{
    /// <summary>
    /// Статические данные уровня: ключ — имя сцены, спавнеры собирает кнопка Collect (LevelStaticDataEditor)
    /// из SpawnMarker на сцене.
    /// </summary>
    [CreateAssetMenu(fileName = "LevelData", menuName = "StaticData/Level")]
    public class LevelStaticData : ScriptableObject
    {
        public string LevelKey;
        public List<EnemySpawnerStaticData> EnemySpawners;
    }
}
