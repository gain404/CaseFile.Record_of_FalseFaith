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

        _dashDirection = stateMachine.MovementInput;
        if (_dashDirection == Vector2.zero)
        {
            _dashDirection = Vector2.right;
        }
        else
        {
            _dashDirection = _dashDirection.normalized;
        }
        
        _rb.AddForce(_dashDirection * stateMachine.DashForce, ForceMode2D.Impulse);
    }

    public override void Exit()
    {
        base.Exit();
        EndAnimation(stateMachine.Player.PlayerAnimationData.DashParameterHash);
    }
}
