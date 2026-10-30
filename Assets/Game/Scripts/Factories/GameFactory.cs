using System.Collections.Generic;
using Game.Scripts.BaseLogic.EnemySpawners;
using Game.Scripts.BaseLogic.Interfaces;
using Game.Scripts.BaseLogic.LootLogic;
using Game.Scripts.Constants;
using Game.Scripts.Enemy;
using Game.Scripts.Factories.Interfaces;
using Game.Scripts.Providers.Interfaces;
using Game.Scripts.Services.PersistentProgress.Interfaces;
using Game.Scripts.Services.Randomizer.Interfaces;
using Game.Scripts.Services.StaticData.Interfaces;
using Game.Scripts.StaticData;
using Game.Scripts.UI.Elements;
using Game.Scripts.UI.Services.Windows.Interfaces;
using UnityEngine;
using UnityEngine.AI;
using Object = UnityEngine.Object;

namespace Game.Scripts.Factories
{
    public class GameFactory : IGameFactory
    {
        private readonly IAssetsProvider _assetsProvider;
        private readonly IStaticDataService _staticData;
        private readonly IRandomService _randomService;
        private readonly IPersistentProgressService _progressService;
        private readonly IWindowService _windowService;

        public List<ISaveProgressReader> ProgressReaders { get; } = new List<ISaveProgressReader>();
        public List<ISaveProgress> ProgressWriters { get; } = new List<ISaveProgress>();

        private GameObject _playerGameObject;

        public GameFactory(IAssetsProvider assetsProvider, IStaticDataService staticData, IRandomService randomService,
            IPersistentProgressService progressService, IWindowService windowService)
        {
            _assetsProvider = assetsProvider;
            _staticData = staticData;
            _randomService = randomService;
            _progressService = progressService;
            _windowService = windowService;
        }

        #region Create

        public GameObject CreatePlayer(GameObject InitialPoint) =>
            _playerGameObject = InstantiateAndRegister(AssetPath.PlayerPath, InitialPoint.transform.position);

        public GameObject CreateHud()
        {
            GameObject hud = InstantiateAndRegister(AssetPath.HudPath);

            hud.GetComponentInChildren<LootCounter>()
                .Construct(_progressService.Progress.WorldData);

            foreach (OpenWindowButton openWindowButton in hud.GetComponentsInChildren<OpenWindowButton>())
                openWindowButton.Init(_windowService);

            return hud;
        }

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

            var lootSpawner = enemy.GetComponentInChildren<EnemyLootSpawner>();
            lootSpawner.SetLoot(enemyData.MinLoot, enemyData.MaxLoot);
            lootSpawner.Construct(this, _randomService);

            var attack = enemy.GetComponent<EnemyAttack>();
            attack.Construct(_playerGameObject.transform);
            attack.Damage = enemyData.Damage;
            attack.Cleavage = enemyData.Cleavage;
            attack.EffectiveDistance = enemyData.EffectiveDistance;

            return enemy;
        }

        public LootPiece CreateLoot()
        {
            var lootPiece = InstantiateAndRegister(AssetPath.Loot).GetComponent<LootPiece>();
            lootPiece.Construct(_progressService.Progress.WorldData);
            return lootPiece;
        }

        public void CreateSpawner(string spawnerId, Vector3 at, EnemyTypeId enemyTypeId)
        {
            SpawnPoint spawner = InstantiateAndRegister(AssetPath.Spawner, at).GetComponent<SpawnPoint>();

            spawner.Construct(this);
            spawner.EnemyTypeId = enemyTypeId;
            spawner.Id = spawnerId;
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

        private void Register(ISaveProgressReader progressReader)
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