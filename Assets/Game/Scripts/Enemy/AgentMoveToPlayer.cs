using Game.Scripts.Data.Extensions;
using Game.Scripts.Factories.Interfaces;
using Game.Scripts.Infrastructure;
using UnityEngine;
using UnityEngine.AI;

namespace Game.Scripts.Enemy
{
    public class AgentMoveToPlayer : Follow
    {
        private const float MinimalDistance = 1;

        public NavMeshAgent Agent;

        private Transform _playerTransform;
        private IGameFactory _gameFactory;

        private void Start()
        {
            _gameFactory = ServiceLocator.Container.Single<IGameFactory>();

            if (_gameFactory.PlayerGameObject != null)
                InitializePlayerTransform();
            else
                _gameFactory.PlayerCreated += OnPlayerCreated;

            EnsureOnNavMesh();
        }

        private void EnsureOnNavMesh()
        {
            if (Agent.isOnNavMesh)
                return;

            if (NavMesh.SamplePosition(Agent.transform.position, out NavMeshHit hit, 5f, NavMesh.AllAreas))
                Agent.Warp(hit.position);
        }

        private void Update()
        {
            if (PlayerTransformInitialized() && PlayerNotReached())
                Agent.destination = _playerTransform.position;
        }

        private void OnDestroy()
        {
            if (_gameFactory != null)
                _gameFactory.PlayerCreated -= OnPlayerCreated;
        }

        private bool PlayerTransformInitialized() =>
            _playerTransform != null;

        private void OnPlayerCreated() =>
            InitializePlayerTransform();

        private void InitializePlayerTransform() =>
            _playerTransform = _gameFactory.PlayerGameObject.transform;

        private bool PlayerNotReached() =>
            Agent.transform.position.SqrMagnitudeTo(_playerTransform.position) >= MinimalDistance;
    }
}
