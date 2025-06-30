using UnityEngine;

public class PlayerDashState : PlayerMoveState
{
    private Vector2 _dashDirection;
    
    public PlayerDashState(PlayerStateMachine playerStateMachine) : base(playerStateMachine)
    {
    }

    public override void Enter()
    {
        StartAnimation(stateMachine.Player.PlayerAnimationData.DashParameterHash);
        base.Enter();
        stateMachine.DashForce = stateMachine.Player.Data.MoveData.DashSpeedModifier;

        Vector2 rawInput = stateMachine.MovementInput;
        if (rawInput != Vector2.zero)
        {
            _dashDirection = rawInput.normalized;
        }
        else
        {
            _dashDirection = stateMachine.Player.PlayerSpriteRenderer.flipX ? Vector2.left: Vector2.right;
        }
        
        _rb.linearVelocity = Vector2.zero;
        _rb.AddForce(_dashDirection * stateMachine.DashForce, ForceMode2D.Impulse);
    }

    public override void Exit()
    {
        base.Exit();
        EndAnimation(stateMachine.Player.PlayerAnimationData.DashParameterHash);
    }

    public override void PhysicsUpdate()
    {
        
    }
}
