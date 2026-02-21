using Game.Scripts.Infrastructure;
using Game.Scripts.Infrastructure.Interfaces;
using Game.Scripts.StateMachine;
using Game.Scripts.StateMachine.States;
using UnityEngine;

namespace Game.Scripts.Core
{
    public class GameBootstrapper : MonoBehaviour, ICoroutineRunner
    {
        public LoadingCurtain LoadingCurtainPrefab;

        private GameMain _game;

        private void Awake()
        {
            _game = new GameMain(this, Instantiate(LoadingCurtainPrefab));
            _game.StateMachine.Enter<BootstrapState>();

            DontDestroyOnLoad(this);
        }
    }
}