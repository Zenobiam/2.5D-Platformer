using UnityEngine;

public class PlayerGroundedState : PlayerBaseState
{
    public PlayerGroundedState(PlayerStateController currentContext, PlayerStateFactory playerStateFactory)
        : base(currentContext, playerStateFactory)
    {
        IsRootState = true;
        OnInitialSubState();
    }

    public override void EnterState()
    {
    }

    public override void OnCheckSwitchStates()
    {
        if (Context.InputController.IsJumpPressed)
            SwitchState(Factory.Jump());
    }

    public override void OnCollisionEnter()
    {
        
    }

    public override void OnExitState()
    {
        
    }

    public override void OnInitialSubState()
    {
        if(Context.IsWalk && !Context.InputController.IsRunPressed)
        {
            SetSubState(Factory.Idle());
        }
        else if (Context.IsWalk)
        {
            SetSubState(Factory.Walk());
        }
        else if(Context.InputController.IsRunPressed)
        {
            SetSubState(Factory.Run());
        }
    }

    public override void UpdateState()
    {
        OnCheckSwitchStates();
    }
}
