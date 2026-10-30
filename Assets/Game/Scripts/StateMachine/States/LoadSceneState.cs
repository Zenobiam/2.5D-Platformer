using System.Collections.Generic;
using Game.Scripts.BaseLogic;
using Game.Scripts.BaseLogic.LootLogic;
using Game.Scripts.Camera;
using Game.Scripts.Data;
using Game.Scripts.Data.Extensions;
using Game.Scripts.Factories.Interfaces;
using Game.Scripts.Player;
using Game.Scripts.Services.PersistentProgress.Interfaces;
using Game.Scripts.Services.StaticData.Interfaces;
using Game.Scripts.StateMachine.Interfaces;
using Game.Scripts.StaticData;
using Game.Scripts.UI.Elements;
using Game.Scripts.UI.Services.Factory.Interfaces;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Game.Scripts.StateMachine.States
{
    public class LoadSceneState : IPayloadedState<string>
    {
        private const string InitialPointTag = "InitialPoint";

        private readonly GameStateMachine _stateMachine;
        private readonly SceneLoader _sceneLoader;
        private readonly LoadingCurtain _loadingCurtain;
        private readonly IGameFactory _gameFactory;
        private IPersistentProgressService _progressService;
        private IStaticDataService _staticData;
        private readonly IUIFactory _uiFactory;

        public LoadSceneState(
            GameStateMachine stateMachine, 
            SceneLoader sceneLoader, 
            LoadingCurtain loadingCurtain, 
            IGameFactory gameFactory, 
            IPersistentProgressService progressService,
            IStaticDataService staticData,
            IUIFactory uiFactory)
        {
            _stateMachine = stateMachine;
            _sceneLoader = sceneLoader;
            _loadingCurtain = loadingCurtain;
            _gameFactory = gameFactory;
            _progressService = progressService;
            _staticData = staticData;
            _uiFactory = uiFactory;
        }

        public void Enter(string sceneName)
        {
            _loadingCurtain.Show();
            _gameFactory.CleanUp();
            _sceneLoader.Load(sceneName, OnLoaded);
        }

        public void Exit() =>
            _loadingCurtain.Hide();

        private void OnLoaded()
        {
            InitUIRoot();
            SetupGameEnvironment();
            InformProgressReaders();

            _stateMachine.Enter<GameLoopState>();
        }

        /// <summary>
        /// UIRoot — первым: окна (магазин) создаются под ним, кнопки HUD могут открыть окно сразу.
        /// </summary>
        private void InitUIRoot() =>
            _uiFactory.CreateUIRoot();

        private void InformProgressReaders()
        {
            foreach (var progressReader in _gameFactory.ProgressReaders)
            {
                progressReader.LoadProgress(_progressService.Progress);
            }
        }

        private void InitHud(GameObject player)
        {
            GameObject hud = _gameFactory.CreateHud();

            hud.GetComponentInChildren<ActorUI>().Construct(player.GetComponent<PlayerHealth>());
        }

        private void SetupGameEnvironment()
        {
            InitSpawners();
            InitLootPieces();

            GameObject player = InitPlayer();
            
            InitHud(player);
            CameraFollow(player);
        }

        /// <summary>
        /// Спавнеры уровня из LevelStaticData (ключ — имя сцены); врагов они создают в LoadProgress,
        /// если спавнер ещё не очищен.
        /// </summary>
        private void InitSpawners()
        {
            string sceneKey = SceneManager.GetActiveScene().name;
            LevelStaticData levelData = _staticData.ForLevel(sceneKey);

            foreach (EnemySpawnerStaticData spawnerData in levelData.EnemySpawners)
                _gameFactory.CreateSpawner(spawnerData.Id, spawnerData.Position, spawnerData.EnemyTypeId);
        }

        /// <summary>
        /// Неподобранный лут из сохранения: создаётся с прежним id, значением и позицией.
        /// </summary>
        private void InitLootPieces()
        {
            foreach (KeyValuePair<string, LootPieceData> item in _progressService.Progress.WorldData.LootData.LootPiecesOnScene.Dictionary)
            {
                LootPiece lootPiece = _gameFactory.CreateLoot();
                lootPiece.GetComponent<UniqueId>().Id = item.Key;
                lootPiece.Initialize(item.Value.Loot);
                lootPiece.transform.position = item.Value.Position.AsVector3();
            }
        }

        private GameObject InitPlayer() => 
            _gameFactory.CreatePlayer(GameObject.FindWithTag(InitialPointTag));

        private static void CameraFollow(GameObject gameObject) =>
            UnityEngine.Camera.main?.GetComponent<CameraController>()?.Follow(gameObject);
    }
}
