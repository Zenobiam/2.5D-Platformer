using Game.Scripts.StaticData;
using UnityEngine;

namespace Game.Scripts.BaseLogic.EnemySpawners
{
    /// <summary>
    /// Метка спавнера на сцене (только для редактора): тип врага + UniqueId рядом.
    /// В игре не работает — LevelStaticDataEditor (Collect) переносит метки в LevelStaticData.
    /// </summary>
    public class SpawnMarker : MonoBehaviour
    {
        public EnemyTypeId EnemyTypeId;
    }
}
