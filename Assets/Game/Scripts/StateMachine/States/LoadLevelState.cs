using Game.Scripts.Factories.Interfaces;
using Game.Scripts.StateMachine.Interfaces;
using UnityEngine;

namespace Game.Scripts.StateMachine.States
{
    public class LoadLevelState : IPayloadedState<string>
    {
        private const string InitialPointTag = "InitialPoint";
        
        private readonly GameStateMachine _stateMachine;
        private readonly SceneLoader _sceneLoader;
        private readonly LoadingCurtain _loadingCurtain;
        private readonly IGameFactory _gameFactory;

        public LoadLevelState(GameStateMachine stateMachine, SceneLoader sceneLoader, LoadingCurtain loadingCurtain)
        {
            _stateMachine = stateMachine;
            _sceneLoader = sceneLoader;
            _loadingCurtain = loadingCurtain;
        }

        public void Enter(string sceneName)
        {
            _loadingCurtain.Show();
            _sceneLoader.Load(sceneName, OnLoaded);
        }

        public void Exit() =>
            _loadingCurtain.Hide();

        private void OnLoaded()
        {
            GameObject player = _gameFactory.CreatePlayer(GameObject.FindWithTag(InitialPointTag));
            _gameFactory.CreateHud();
            CameraFollow(player);

            _stateMachine.Enter<GameLoopState>();
        }

        private static void CameraFollow(GameObject gameObject) =>
            Camera.main?.GetComponent<CameraController>()?.Follow(gameObject);
    }
}
