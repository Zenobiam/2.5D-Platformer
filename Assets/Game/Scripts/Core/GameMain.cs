using Game.Scripts.Infrastructure;
using Game.Scripts.Infrastructure.Interfaces;
using Game.Scripts.Services.Input;
using Game.Scripts.Services.Input.Interfaces;
using Game.Scripts.StateMachine;

namespace Game.Scripts.Core
{
    public class GameMain
    {
        public static IInputService InputService;
        public GameStateMachine StateMachine;

        public GameMain(ICoroutineRunner coroutineRunner, LoadingCurtain loadingCurtain)
        {
            StateMachine = new GameStateMachine(new SceneLoader(coroutineRunner), loadingCurtain);
        }
    }
}