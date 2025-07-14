
public class PlayerWalkState : PlayerMoveState
{
    public PlayerWalkState(PlayerStateMachine playerStateMachine) : base(playerStateMachine)
    {
    }

    public override void Enter()
    {
        stateMachine.MovementSpeedModifier = moveData.WalkSpeedModifier;
        base.Enter();
        StartAnimation(stateMachine.Player.PlayerAnimationData.WalkParameterHash);
    }

    public override void Exit()
    {
        base.Exit();
        EndAnimation(stateMachine.Player.PlayerAnimationData.WalkParameterHash);
    }
}
