using Game.Scripts.StateMachine.Interfaces;
using Game.Scripts.StateMachine.States;
using UnityEngine;

namespace Game.Scripts.Triggers
{
    /// <summary>
    /// Переход на другой уровень (урок 15): игрок входит в триггер → LoadSceneState со сценой TransferTo.
    /// Создаёт GameFactory.CreateLevelTransfer: позиция и TransferTo — из LevelStaticData (в курсе TransferTo — поле префаба).
    /// </summary>
    public class LevelTransferTrigger : MonoBehaviour
    {
        private const string PlayerTag = "Player";

        public string TransferTo;
        public BoxCollider Collider;

        private IGameStateMachine _stateMachine;

        /// <summary>
        /// Триггер срабатывает один раз: игрок стоит в нём, пока грузится уровень, — без флага загрузка пошла бы повторно.
        /// </summary>
        private bool _triggered;

        public void Construct(IGameStateMachine stateMachine) =>
            _stateMachine = stateMachine;

        private void OnTriggerEnter(Collider other)
        {
            if (_triggered)
                return;

            if (other.CompareTag(PlayerTag))
            {
                _stateMachine.Enter<LoadSceneState, string>(TransferTo);
                _triggered = true;
            }
        }

        private void OnDrawGizmos()
        {
            if (!Collider) return;

            Gizmos.color = new Color32(30, 30, 200, 130);
            Gizmos.DrawCube(transform.position + Collider.center, Collider.size);
        }
    }
}
