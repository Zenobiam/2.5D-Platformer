using System.Collections.Generic;
using Game.Scripts.BaseLogic.LootLogic;
using Game.Scripts.Infrastructure;
using Game.Scripts.Services.PersistentProgress.Interfaces;
using Game.Scripts.StaticData;
using UnityEngine;

namespace Game.Scripts.Factories.Interfaces
{
    public interface IGameFactory : IService
    {
        GameObject CreatePlayer(GameObject InitialPoint);
        GameObject CreateHud();
        List<ISaveProgressReader> ProgressReaders { get; }
        List<ISaveProgress> ProgressWriters { get; }
        void CleanUp();
        GameObject CreateEnemy(EnemyTypeId typeId, Transform parent);
        LootPiece CreateLoot();
        void CreateSpawner(string spawnerId, Vector3 at, EnemyTypeId enemyTypeId);
    }
}
