using Game.Scripts.Core;
using Game.Scripts.Factories;
using Game.Scripts.Factories.Interfaces;
using Game.Scripts.Infrastructure;
using Game.Scripts.Providers;
using Game.Scripts.Providers.Interfaces;
using Game.Scripts.Services.Input;
using Game.Scripts.Services.Input.Interfaces;
using Game.Scripts.Services.PersistentProgress;
using Game.Scripts.Services.PersistentProgress.Interfaces;
using Game.Scripts.Services.SaveLoad.Interfaces;
using Game.Scripts.StateMachine.Interfaces;

namespace Game.Scripts.StateMachine.States
{
    public class BootstrapState : IState
    {
        private const string InitialScene = "InitialScene";
        private readonly GameStateMachine _stateMachine;
        private readonly SceneLoader _sceneLoader;
        private ServiceLocator _service;

        public BootstrapState(GameStateMachine stateMachine, SceneLoader sceneLoader, ServiceLocator services)
        {
            _stateMachine = stateMachine;
            _sceneLoader = sceneLoader;
            _service = services;

            RegisterServices();
        }

        public void Enter()
        {
            _sceneLoader.Load(sceneName: InitialScene, onLoaded: EnterLoadLevel);
        }

        public void Exit()
        {
        }

        private void EnterLoadLevel() =>
            _stateMachine.Enter<LoadProgressState>();

        private void RegisterServices()
        {
            _service.RegisterSingle<IInputService>(InputService());
            _service.RegisterSingle<IAssetsProvider>(new AssetsProvider());
            _service.RegisterSingle<IPersistentProgressService>(new PersistentProgressService());
            _service.RegisterSingle<IGameFactory>(new GameFactory(_service.Single<IAssetsProvider>()));
            _service.RegisterSingle<ISaveLoadService>(new SaveLoadService(_service.Single<IPersistentProgressService>(), _service.Single<IGameFactory>()));
        }

        private static IInputService InputService()
        {
            return new InputService();
        }
    }
}