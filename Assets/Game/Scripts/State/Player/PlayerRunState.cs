using UnityEngine;

public class PlayerRunState : PlayerBaseState
{
    public PlayerRunState(PlayerStateController currentContext, PlayerStateFactory playerStateFactory)
        : base(currentContext, playerStateFactory)
    {

    }
    public override void EnterState()
    {
        
    }

    public override void OnCheckSwitchStates()
    {
        if (Context.IsWalk && !Context.InputController.IsRunPressed)
        {
            SetSubState(Factory.Idle());
        }
        else if (Context.IsWalk)
        {
            SetSubState(Factory.Walk());
        }
    }

    public override void OnCollisionEnter()
    {
        
    }

    public override void OnExitState()
    {
        
    }

    public override void OnInitialSubState()
    {
        
    }

    public override void UpdateState()
    {
        OnCheckSwitchStates();
    }
}
