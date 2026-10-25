using System;
using Game.Scripts.Data;
using Game.Scripts.Enemy;
using Game.Scripts.Factories.Interfaces;
using Game.Scripts.Infrastructure;
using Game.Scripts.Services.PersistentProgress.Interfaces;
using Game.Scripts.StaticData;
using Unity.Android.Gradle.Manifest;
using UnityEngine;

namespace Game.Scripts.BaseLogic.Enemy
{
    public class EnemySpawner : MonoBehaviour, ISaveProgress
    {
        public EnemyTypeId EnemyTypeId;
        private string _id;

        public bool isEmpty;
        private IGameFactory _factory;
        private EnemyDeath _enemyDeath;

        private void Awake()
        {
            _id = GetComponent<UniqueId>().Id;
            _factory = ServiceLocator.Container.Single<IGameFactory>();
        }

        public void LoadProgress(PlayerProgress progress)
        {
            if(progress.KillData.ClearedSpawners.Contains(_id))
                isEmpty = true;
            else
                Spawn();
        }

        private void Spawn()
        {
            GameObject enemy = _factory.CreateEnemy(EnemyTypeId, transform);
            _enemyDeath = enemy.GetComponent<EnemyDeath>();
            enemy.GetComponent<EnemyDeath>().DeathAction += OnDeath;
        }

        private void OnDeath()
        {
            if (_enemyDeath  != null)
                _enemyDeath.DeathAction -= OnDeath;
            
            isEmpty = true;
        }

        public void UpdateProgress(PlayerProgress progress)
        {
           if(isEmpty)
               progress.KillData.ClearedSpawners.Add(_id);
        }
    }
}