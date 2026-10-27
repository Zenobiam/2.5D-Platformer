using Game.Scripts.BaseLogic.Enemy;
using Game.Scripts.BaseLogic.LootLogic;
using Game.Scripts.Camera;
using Game.Scripts.Factories.Interfaces;
using Game.Scripts.Player;
using Game.Scripts.Services.PersistentProgress.Interfaces;
using Game.Scripts.StateMachine.Interfaces;
using Game.Scripts.UI;
using UnityEngine;

namespace Game.Scripts.StateMachine.States
{
    public class LoadSceneState : IPayloadedState<string>
    {
        private const string InitialPointTag = "InitialPoint";
        private const string EnemySpawnerTag = "EnemySpawner";

        private readonly GameStateMachine _stateMachine;
        private readonly SceneLoader _sceneLoader;
        private readonly LoadingCurtain _loadingCurtain;
        private readonly IGameFactory _gameFactory;
        private IPersistentProgressService _progressService;

        public LoadSceneState(GameStateMachine stateMachine, SceneLoader sceneLoader, LoadingCurtain loadingCurtain, IGameFactory gameFactory, IPersistentProgressService progressService)
        {
            _stateMachine = stateMachine;
            _sceneLoader = sceneLoader;
            _loadingCurtain = loadingCurtain;
            _gameFactory = gameFactory;
            _progressService = progressService;
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
            SetupGameEnvironment();
            InformProgressReaders();

            _stateMachine.Enter<GameLoopState>();
        }

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

        private void InitSpawners()
        {
            foreach (GameObject spawnerObject in GameObject.FindGameObjectsWithTag(EnemySpawnerTag))
            {
                var spawner = spawnerObject.GetComponent<EnemySpawner>();
                _gameFactory.Register(spawner);
            }
        }

        /// <summary>
        /// Неподобранный лут из сохранения: создаётся с прежним id, значение и позицию берёт в LoadProgress
        /// (его вызывает InformProgressReaders после SetupGameEnvironment).
        /// </summary>
        private void InitLootPieces()
        {
            foreach (string key in _progressService.Progress.WorldData.LootData.LootPiecesOnScene.Dictionary.Keys)
            {
                LootPiece lootPiece = _gameFactory.CreateLoot();
                lootPiece.GetComponent<UniqueId>().Id = key;
            }
        }

        private GameObject InitPlayer() => 
            _gameFactory.CreatePlayer(GameObject.FindWithTag(InitialPointTag));

        private static void CameraFollow(GameObject gameObject) =>
            UnityEngine.Camera.main?.GetComponent<CameraController>()?.Follow(gameObject);
    }
}
