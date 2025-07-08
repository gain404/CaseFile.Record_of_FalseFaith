

public class PlayerRunState : PlayerMoveState
{
    public PlayerRunState(PlayerStateMachine playerStateMachine) : base(playerStateMachine)
    {
    }

    public override void Enter()
    {
        base.Enter();
        stateMachine.MovementSpeedModifier = moveData.RunSpeedModifier;
        StartAnimation(stateMachine.Player.PlayerAnimationData.RunParameterHash);
        CameraController.Instance.ZoomOutForRunning();
    }

    public override void Exit()
    {
        base.Exit();
        EndAnimation(stateMachine.Player.PlayerAnimationData.RunParameterHash);
        CameraController.Instance.ZoomInToDefault();
    }
    
}
