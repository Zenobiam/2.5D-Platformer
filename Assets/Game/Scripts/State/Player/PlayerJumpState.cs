using UnityEngine;

public class PlayerJumpState : PlayerBaseState
{
    private int _currentJumps = 0;

    public PlayerJumpState(PlayerStateController context, PlayerStateFactory factory) 
        : base(context, factory)
    {
        IsRootState = true;
        OnInitialSubState();
    }

    public override void EnterState()
    {
        HandleJump();
    }

    public override void OnCheckSwitchStates()
    {
        if (Context.IsGrounded)
            SwitchState(Factory.Grounded());
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

    private void HandleJump()
    {       
        Context.Rigidbody.linearVelocity = new Vector2(Context.Rigidbody.linearVelocity.x, Context.JumpForce);
        _currentJumps++;
    }
}
