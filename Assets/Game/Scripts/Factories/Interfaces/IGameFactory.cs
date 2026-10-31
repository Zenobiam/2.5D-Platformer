using System.Collections.Generic;
using System.Threading.Tasks;
using Game.Scripts.BaseLogic.LootLogic;
using Game.Scripts.Infrastructure;
using Game.Scripts.Services.PersistentProgress.Interfaces;
using Game.Scripts.StaticData;
using UnityEngine;

namespace Game.Scripts.Factories.Interfaces
{
    public interface IGameFactory : IService
    {
        Task WarmUp();
        Task<GameObject> CreatePlayer(Vector3 at);
        Task<GameObject> CreateHud();
        Task CreateLevelTransfer(Vector3 at, string transferTo);
        List<ISaveProgressReader> ProgressReaders { get; }
        List<ISaveProgress> ProgressWriters { get; }
        void CleanUp();
        Task<GameObject> CreateEnemy(EnemyTypeId typeId, Transform parent);
        Task<LootPiece> CreateLoot();
        Task CreateSpawner(string spawnerId, Vector3 at, EnemyTypeId enemyTypeId);
    }
}
