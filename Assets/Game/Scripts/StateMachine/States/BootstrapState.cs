using Game.Scripts.Core;
using Game.Scripts.Services.Input;
using Game.Scripts.Services.Input.Interfaces;
using Game.Scripts.StateMachine.Interfaces;

namespace Game.Scripts.StateMachine.States
{
    public class BootstrapState : IState
    {
        private const string InitialScene = "InitialScene";
        private readonly GameStateMachine _stateMachine;
        private readonly SceneLoader _sceneLoader;

        public BootstrapState(GameStateMachine stateMachine, SceneLoader sceneLoader)
        {
            _stateMachine = stateMachine;
            _sceneLoader = sceneLoader;
        }

        public void Enter()
        {
            RegisterServices();
            _sceneLoader.Load(sceneName: InitialScene, onLoaded: EnterLoadLevel);
        }

        private void EnterLoadLevel() =>
            _stateMachine.Enter<LoadLevelState, string>("SampleScene");

        private void RegisterServices()
        {
            GameMain.InputService = RegisterInputService();
        }

        public void Exit()
        {
        }

        private static IInputService RegisterInputService()
        {
            return new InputService();
        }
    }
}
