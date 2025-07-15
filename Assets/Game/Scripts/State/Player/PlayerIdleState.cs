using UnityEngine;

public class PlayerIdleState : PlayerBaseState
{
    public PlayerIdleState(PlayerStateController currentContext, PlayerStateFactory playerStateFactory)
        : base(currentContext, playerStateFactory)
    {

    }
    public override void EnterState()
    {
        
    }

    public override void OnCheckSwitchStates()
    {
        if (Context.IsWalk)
        {
            SwitchState(Factory.Walk());
        }
        else if (Context.InputController.IsRunPressed)
        {
            SwitchState(Factory.Run());
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
