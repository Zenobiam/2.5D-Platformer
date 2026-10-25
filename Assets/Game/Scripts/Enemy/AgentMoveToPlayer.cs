using UnityEngine;
using UnityEngine.AI;

namespace Game.Scripts.Enemy
{
    public class AgentMoveToPlayer : Follow
    {
        public NavMeshAgent Agent;

        private Transform _playerTransform;

        public void Construct(Transform playerTransform) =>
            _playerTransform = playerTransform;

        // Не из курса: враг появляется в позиции спавнера, а уровень не на y = 0 — агент может оказаться вне NavMesh.
        private void Start() =>
            EnsureOnNavMesh();

        private void Update() =>
            SetDestinationForAgent();

        private void SetDestinationForAgent()
        {
            if (_playerTransform)
                Agent.destination = _playerTransform.position;
        }

        private void EnsureOnNavMesh()
        {
            if (Agent.isOnNavMesh)
                return;

            if (NavMesh.SamplePosition(Agent.transform.position, out NavMeshHit hit, 5f, NavMesh.AllAreas))
                Agent.Warp(hit.position);
        }
    }
}
