using UnityEngine;

public class PlayerCrouchState : PlayerBaseState
{
    public PlayerCrouchState(PlayerStateController currentContext, PlayerStateFactory playerStateFactory)
        : base(currentContext, playerStateFactory)
    {

    }
    public override void EnterState()
    {
        
    }

    public override void OnCheckSwitchStates()
    {
        
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
