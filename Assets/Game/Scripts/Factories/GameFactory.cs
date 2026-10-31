using System.Collections.Generic;
using System.Threading.Tasks;
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
using Game.Scripts.StateMachine.Interfaces;
using Game.Scripts.StaticData;
using Game.Scripts.Triggers;
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
        private readonly IGameStateMachine _stateMachine;

        public List<ISaveProgressReader> ProgressReaders { get; } = new List<ISaveProgressReader>();
        public List<ISaveProgress> ProgressWriters { get; } = new List<ISaveProgress>();

        private GameObject _playerGameObject;

        public GameFactory(IAssetsProvider assetsProvider, IStaticDataService staticData, IRandomService randomService,
            IPersistentProgressService progressService, IWindowService windowService, IGameStateMachine stateMachine)
        {
            _assetsProvider = assetsProvider;
            _staticData = staticData;
            _randomService = randomService;
            _progressService = progressService;
            _windowService = windowService;
            _stateMachine = stateMachine;
        }

        /// <summary>
        /// Прогрев (урок 15): лут и спавнер загружаются заранее, пока грузится сцена, — первый CreateLoot берёт их из кэша.
        /// </summary>
        public async Task WarmUp()
        {
            await _assetsProvider.Load<GameObject>(AssetAddress.Loot);
            await _assetsProvider.Load<GameObject>(AssetAddress.Spawner);
        }

        #region Create

        public async Task<GameObject> CreatePlayer(Vector3 at) =>
            _playerGameObject = await InstantiateAndRegisterAsync(AssetAddress.PlayerPath, at);

        /// <summary>
        /// В курсе TransferTo берётся из поля префаба — один префаб ведёт всех на одну сцену.
        /// У нас два уровня ведут друг в друга, поэтому TransferTo — из LevelStaticData (course-deviations.md).
        /// </summary>
        public async Task CreateLevelTransfer(Vector3 at, string transferTo)
        {
            GameObject prefab = await InstantiateAndRegisterAsync(AssetAddress.LevelTransferTrigger, at);
            LevelTransferTrigger levelTransfer = prefab.GetComponent<LevelTransferTrigger>();

            levelTransfer.Construct(_stateMachine);
            levelTransfer.TransferTo = transferTo;
        }

        public async Task<GameObject> CreateHud()
        {
            GameObject hud = await InstantiateAndRegisterAsync(AssetAddress.HudPath);

            hud.GetComponentInChildren<LootCounter>()
                .Construct(_progressService.Progress.WorldData);

            foreach (OpenWindowButton openWindowButton in hud.GetComponentsInChildren<OpenWindowButton>())
                openWindowButton.Init(_windowService);

            return hud;
        }

        public async Task<GameObject> CreateEnemy(EnemyTypeId typeId, Transform parent)
        {
            EnemyStaticData enemyData = _staticData.ForEnemy(typeId);

            GameObject prefab = await _assetsProvider.Load<GameObject>(enemyData.PrefabReference);
            GameObject enemy = Object.Instantiate(prefab, parent.position, Quaternion.identity, parent);

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

        public async Task<LootPiece> CreateLoot()
        {
            GameObject prefab = await _assetsProvider.Load<GameObject>(AssetAddress.Loot);
            var lootPiece = InstantiateAndRegister(prefab).GetComponent<LootPiece>();

            lootPiece.Construct(_progressService.Progress.WorldData);
            return lootPiece;
        }

        public async Task CreateSpawner(string spawnerId, Vector3 at, EnemyTypeId enemyTypeId)
        {
            GameObject prefab = await _assetsProvider.Load<GameObject>(AssetAddress.Spawner);
            SpawnPoint spawner = InstantiateAndRegister(prefab, at).GetComponent<SpawnPoint>();

            spawner.Construct(this);
            spawner.EnemyTypeId = enemyTypeId;
            spawner.Id = spawnerId;
        }

        #endregion

        #region Instantiate

        /// <summary>
        /// Префаб уже загружен (Load, с кэшем) — создаём обычным Instantiate: так делает курс для лута, спавнера и врагов.
        /// </summary>
        private GameObject InstantiateAndRegister(GameObject prefab, Vector3 position)
        {
            GameObject gameObject = Object.Instantiate(prefab, position, Quaternion.identity);
            RegisterProgressWatchers(gameObject);
            return gameObject;
        }

        private GameObject InstantiateAndRegister(GameObject prefab)
        {
            GameObject gameObject = Object.Instantiate(prefab);
            RegisterProgressWatchers(gameObject);
            return gameObject;
        }

        /// <summary>
        /// Загрузка и создание одним вызовом (Addressables.InstantiateAsync): игрок, HUD, переход.
        /// </summary>
        private async Task<GameObject> InstantiateAndRegisterAsync(string address, Vector3 position)
        {
            GameObject gameObject = await _assetsProvider.Instantiate(address, at: position);
            RegisterProgressWatchers(gameObject);
            return gameObject;
        }

        private async Task<GameObject> InstantiateAndRegisterAsync(string address)
        {
            GameObject gameObject = await _assetsProvider.Instantiate(address);
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

            _assetsProvider.CleanUp();
        }
    }
}
