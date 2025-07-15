using UnityEngine;

public class PlayerWalkState : PlayerBaseState
{
    public PlayerWalkState(PlayerStateController currentContext, PlayerStateFactory playerStateFactory)
        :base(currentContext, playerStateFactory)
    {

    }
    public override void EnterState()
    {
        
    }

    public override void OnCheckSwitchStates()
    {
        if (Context.IsWalk && !Context.InputController.IsRunPressed)
        {
            SwitchState(Factory.Idle());
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
