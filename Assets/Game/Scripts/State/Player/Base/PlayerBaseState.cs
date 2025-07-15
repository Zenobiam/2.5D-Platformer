using UnityEngine;

public abstract class PlayerBaseState
{
    private PlayerStateController _context;
    private PlayerStateFactory _factory;
    private PlayerBaseState _currentSuperState;
    private PlayerBaseState _currentSubState;

    private bool _isRootState = false;

    protected PlayerStateController Context { get { return _context; } }
    protected PlayerStateFactory Factory { get { return _factory; } }
    protected bool IsRootState { set { _isRootState = value; } }

    public PlayerBaseState(PlayerStateController context, PlayerStateFactory factory)
    {
        _context = context;
        _factory = factory;
    }

    public abstract void EnterState();
    public abstract void UpdateState();
    public abstract void OnExitState();
    public abstract void OnCheckSwitchStates();
    public abstract void OnInitialSubState();
    public abstract void OnCollisionEnter();


    public void UpdateStates()
    {
        UpdateState();
        if (_currentSubState != null)
            _currentSubState.UpdateState();
    }
    protected void SwitchState(PlayerBaseState newState)
    {
        Debug.Log($"Switch state " +
            $"OLD - {_context.CurrentState.ToString()}, " +
            $"NEW - {newState.ToString()}");

        OnExitState();

        newState.EnterState();

        if ( _isRootState )
            _context.CurrentState = newState; // ѕереключает состо€ние только если оно €вл€етс€ корневым(главным с цепочке, например PlayerGroundedState)
        else if (_currentSuperState != null)
            _currentSuperState.SetSubState(newState); // ѕередает подстосто€ние при наличии у него того суперсосто€ни€
    }
    protected void SetSuperState(PlayerBaseState newSuperState)
    {
        _currentSuperState = newSuperState;
    }
    protected void SetSubState(PlayerBaseState newSubState)
    {
        _currentSubState = newSubState;
        newSubState.SetSuperState(this);
    }
}
