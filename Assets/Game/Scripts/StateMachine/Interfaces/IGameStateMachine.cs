using Game.Scripts.Infrastructure;

namespace Game.Scripts.StateMachine.Interfaces
{
    /// <summary>
    /// Машина состояний как сервис (урок 15): её получает фабрика, чтобы передать триггеру перехода между уровнями.
    /// </summary>
    public interface IGameStateMachine : IService
    {
        void Enter<TState>() where TState : class, IState;
        void Enter<TState, TPayload>(TPayload payload) where TState : class, IPayloadedState<TPayload>;
    }
}
