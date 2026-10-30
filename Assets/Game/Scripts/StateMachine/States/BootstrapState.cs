using Game.Scripts.Factories;
using Game.Scripts.Factories.Interfaces;
using Game.Scripts.Infrastructure;
using Game.Scripts.Providers;
using Game.Scripts.Providers.Interfaces;
using Game.Scripts.Services.Input;
using Game.Scripts.Services.Input.Interfaces;
using Game.Scripts.Services.PersistentProgress;
using Game.Scripts.Services.PersistentProgress.Interfaces;
using Game.Scripts.Services.Randomizer;
using Game.Scripts.Services.Randomizer.Interfaces;
using Game.Scripts.Services.SaveLoad;
using Game.Scripts.Services.SaveLoad.Interfaces;
using Game.Scripts.Services.StaticData;
using Game.Scripts.Services.StaticData.Interfaces;
using Game.Scripts.StateMachine.Interfaces;
using Game.Scripts.UI.Services.Factory;
using Game.Scripts.UI.Services.Factory.Interfaces;
using Game.Scripts.UI.Services.Windows;
using Game.Scripts.UI.Services.Windows.Interfaces;

namespace Game.Scripts.StateMachine.States
{
    public class BootstrapState : IState
    {
        private const string InitialScene = "InitialScene";
        private readonly GameStateMachine _stateMachine;
        private readonly SceneLoader _sceneLoader;
        private readonly ServiceLocator _services;

        public BootstrapState(GameStateMachine stateMachine, SceneLoader sceneLoader, ServiceLocator serviceses)
        {
            _stateMachine = stateMachine;
            _sceneLoader = sceneLoader;
            _services = serviceses;

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
            RegisterStaticData();

            _services.RegisterSingle<IInputService>(InputService());
            _services.RegisterSingle<IAssetsProvider>(new AssetsProvider());
            _services.RegisterSingle<IRandomService>(new RandomService());
            _services.RegisterSingle<IPersistentProgressService>(new PersistentProgressService());
            _services.RegisterSingle<IUIFactory>(new UIFactory(
                _services.Single<IAssetsProvider>(),
                _services.Single<IStaticDataService>(),
                _services.Single<IPersistentProgressService>()));
            _services.RegisterSingle<IWindowService>(new WindowService(_services.Single<IUIFactory>()));
            _services.RegisterSingle<IGameFactory>(new GameFactory(
                _services.Single<IAssetsProvider>(),
                _services.Single<IStaticDataService>(),
                _services.Single<IRandomService>(),
                _services.Single<IPersistentProgressService>(),
                _services.Single<IWindowService>()));
            _services.RegisterSingle<ISaveLoadService>(new SaveLoadService(_services.Single<IPersistentProgressService>(), _services.Single<IGameFactory>()));
        }

        private void RegisterStaticData()
        {
            IStaticDataService staticData = new StaticDataService();
            staticData.Load();
            _services.RegisterSingle<IStaticDataService>(staticData);
        }

        private static IInputService InputService()
        {
            return new InputService();
        }
    }
}