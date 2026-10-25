using System.Collections.Generic;
using Game.Scripts.BaseLogic.Interfaces;
using Game.Scripts.Constants;
using Game.Scripts.Enemy;
using Game.Scripts.Factories.Interfaces;
using Game.Scripts.Providers.Interfaces;
using Game.Scripts.Services.PersistentProgress.Interfaces;
using Game.Scripts.Services.StaticData.Interfaces;
using Game.Scripts.StaticData;
using Game.Scripts.UI;
using UnityEngine;
using UnityEngine.AI;
using Object = UnityEngine.Object;

namespace Game.Scripts.Factories
{
    public class GameFactory : IGameFactory
    {
        private readonly IAssetsProvider _assetsProvider;
        private readonly IStaticDataService _staticData;

        public List<ISaveProgressReader> ProgressReaders { get; } = new List<ISaveProgressReader>();
        public List<ISaveProgress> ProgressWriters { get; } = new List<ISaveProgress>();

        private GameObject _playerGameObject;

        public GameFactory(IAssetsProvider assetsProvider, IStaticDataService staticData)
        {
            _assetsProvider = assetsProvider;
            _staticData = staticData;
        }

        #region Create

        public GameObject CreatePlayer(GameObject InitialPoint) =>
            _playerGameObject = InstantiateAndRegister(AssetPath.PlayerPath, InitialPoint.transform.position);

        public GameObject CreateHud() =>
            InstantiateAndRegister(AssetPath.HudPath);

        public GameObject CreateEnemy(EnemyTypeId typeId, Transform parent)
        {
            EnemyStaticData enemyData = _staticData.ForEnemy(typeId);
            GameObject enemy = Object.Instantiate(enemyData.Prefab, parent.position, Quaternion.identity, parent);
            var health = enemy.GetComponent<IHealth>();
            health.Current = enemyData.Hp;
            health.Max = enemyData.Hp;
            
            enemy.GetComponent<ActorUI>().Construct(health);
            enemy.GetComponent<AgentMoveToPlayer>().Construct(_playerGameObject.transform);
            // RotateToHero есть не у каждого врага (сейчас ни у одного) — как в курсе, через ?.
            enemy.GetComponent<RotateToPlayer>()?.Construct(_playerGameObject.transform);
            enemy.GetComponent<NavMeshAgent>().speed = enemyData.MoveSpeed;

            var attack = enemy.GetComponent<EnemyAttack>();
            attack.Construct(_playerGameObject.transform);
            attack.Damage = enemyData.Damage;
            attack.Cleavage = enemyData.Cleavage;
            attack.EffectiveDistance = enemyData.EffectiveDistance;

            return enemy;
        }

        #endregion

        #region Instantiate

        private GameObject InstantiateAndRegister(string prefabPath, Vector3 position)
        {
            GameObject gameObject = _assetsProvider.Instantiate(prefabPath, at: position);
            RegisterProgressWatchers(gameObject);
            return gameObject;
        }

        private GameObject InstantiateAndRegister(string prefabPath)
        {
            GameObject gameObject = _assetsProvider.Instantiate(prefabPath);
            RegisterProgressWatchers(gameObject);
            return gameObject;
        }

        #endregion

        #region Register

        private void RegisterProgressWatchers(GameObject gameObject)
        {
            foreach (ISaveProgressReader progressReader in gameObject.GetComponentsInChildren<ISaveProgressReader>())
                Register(progressReader);
        }

        public void Register(ISaveProgressReader progressReader)
        {
            if (progressReader is ISaveProgress progressWriter)
                ProgressWriters.Add(progressWriter);

            ProgressReaders.Add(progressReader);
        }

        #endregion

        public void CleanUp()
        {
            ProgressWriters.Clear();
            ProgressReaders.Clear();
        }
    }
}