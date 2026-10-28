using System.Collections.Generic;
using Game.Scripts.Data;
using Game.Scripts.Enemy;
using Game.Scripts.Factories.Interfaces;
using Game.Scripts.Services.PersistentProgress.Interfaces;
using Game.Scripts.StaticData;
using UnityEngine;

namespace Game.Scripts.BaseLogic.EnemySpawners
{
    /// <summary>
    /// Точка спавна в игре: создаёт GameFactory.CreateSpawner по LevelStaticData.
    /// Если спавнер в прогрессе уже очищен — врага не создаёт.
    /// </summary>
    public class SpawnPoint : MonoBehaviour, ISaveProgress
    {
        public EnemyTypeId EnemyTypeId;

        public string Id { get; set; }

        private IGameFactory _factory;
        private EnemyDeath _enemyDeath;
        private bool _slain;

        public void Construct(IGameFactory gameFactory) =>
            _factory = gameFactory;

        private void OnDestroy()
        {
            if (_enemyDeath != null)
                _enemyDeath.Died -= Slay;
        }

        public void LoadProgress(PlayerProgress progress)
        {
            if (progress.KillData.ClearedSpawners.Contains(Id))
                _slain = true;
            else
                Spawn();
        }

        public void UpdateProgress(PlayerProgress progress)
        {
            List<string> slainSpawners = progress.KillData.ClearedSpawners;

            if (_slain && !slainSpawners.Contains(Id))
                slainSpawners.Add(Id);
        }

        private void Spawn()
        {
            GameObject enemy = _factory.CreateEnemy(EnemyTypeId, transform);
            _enemyDeath = enemy.GetComponent<EnemyDeath>();
            _enemyDeath.Died += Slay;
        }

        private void Slay()
        {
            if (_enemyDeath != null)
                _enemyDeath.Died -= Slay;

            _slain = true;
        }
    }
}
