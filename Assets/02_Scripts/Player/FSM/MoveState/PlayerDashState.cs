using UnityEngine;

public class PlayerDashState : PlayerMoveState
{
    public PlayerDashState(PlayerStateMachine playerStateMachine) : base(playerStateMachine)
    {
    }

    public override void Enter()
    {
        base.Enter();
        stateMachine.DashForce = stateMachine.Player.Data.MoveData.DashSpeedModifier;
        StartAnimation(stateMachine.Player.PlayerAnimationData.DashParameterHash);
    }

    public override void Exit()
    {
        base.Exit();
        EndAnimation(stateMachine.Player.PlayerAnimationData.DashParameterHash);
    }

    private void OnDash()
    {
        float dashSpeed = stateMachine.MovementSpeed * stateMachine.DashForce;
        _rb.linearVelocity = new Vector2(stateMachine.MovementInput.x * dashSpeed, _rb.linearVelocity.y);
    }

    public override void PhysicsUpdate()
    {
        OnDash();
    }
}
