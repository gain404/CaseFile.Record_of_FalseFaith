

public class PlayerRunState : PlayerMoveState
{
    public PlayerRunState(PlayerStateMachine playerStateMachine) : base(playerStateMachine)
    {
    }

    public override void Enter()
    {
        stateMachine.MovementSpeedModifier = 3f;
        base.Enter();
        StartAnimation(stateMachine.Player.PlayerAnimationData.RunParameterHash);
    }

    public override void Exit()
    {
        stateMachine.MovementSpeedModifier = 1f;
        base.Exit();
        EndAnimation(stateMachine.Player.PlayerAnimationData.RunParameterHash);
    }
    
}
