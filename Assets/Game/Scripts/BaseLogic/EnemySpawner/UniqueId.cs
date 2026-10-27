using System;
using UnityEngine;

namespace Game.Scripts.BaseLogic.Enemy
{
    public class UniqueId : MonoBehaviour
    {
        public string Id;

        /// <summary>
        /// Id вида «сцена_guid». Вызывают UniqueIdEditor (объекты на сцене) и EnemyLootSpawner (лут, созданный в игре).
        /// </summary>
        public void GenerateId() =>
            Id = $"{gameObject.scene.name}_{Guid.NewGuid().ToString()}";
    }
}
