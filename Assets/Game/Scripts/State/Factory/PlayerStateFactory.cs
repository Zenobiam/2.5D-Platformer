using UnityEngine;

/// <summary>
/// Получение объекта состояния
/// </summary>
public class PlayerStateFactory
{
    PlayerStateController _context;

    public PlayerStateFactory(PlayerStateController currentContext)
    {
        _context = currentContext;
    }

    public PlayerBaseState Idle()
    {
        return new PlayerIdleState(_context, this);
    }
    public PlayerBaseState Grounded()
    {
        return new PlayerGroundedState(_context, this);
    }
    public PlayerBaseState Air()
    {
        return new PlayerAirState(_context, this);
    }
    public PlayerBaseState Crouch()
    {
        return new PlayerCrouchState(_context, this);
    }
    public PlayerBaseState Run()
    {
        return new PlayerRunState(_context, this);
    }
    public PlayerBaseState Walk()
    {
        return new PlayerWalkState(_context, this);
    }
    public PlayerBaseState Death()
    {
        return new PlayerDeathState(_context, this);
    }
    public PlayerBaseState Jump()
    {
        return new PlayerJumpState(_context, this);
    }
}
