
public class PlayerRunState : PlayerMoveState
{
    private CameraTargetMover cameraTargetMover;
    public PlayerRunState(PlayerStateMachine playerStateMachine) : base(playerStateMachine)
    {
    }

    public override void Enter()
    {
        base.Enter();
        if (cameraTargetMover == null)
        {
            cameraTargetMover = stateMachine.Player.transform.Find("CameraTarget").GetComponent<CameraTargetMover>();
        }

        cameraTargetMover.MoveToOffset(new UnityEngine.Vector3(0f, 0.7f, 0f), 0.5f); // 예시: 위로 0.7만큼 0.5초 동안
        stateMachine.MovementSpeedModifier = moveData.RunSpeedModifier;
        StartAnimation(stateMachine.Player.PlayerAnimationData.RunParameterHash);
        CameraController.Instance.ZoomOutForRunning();
        CameraController.Instance.isRunning = true;
    }

    public override void Exit()
    {
        base.Exit();
        cameraTargetMover.ResetPosition(0.3f);
        EndAnimation(stateMachine.Player.PlayerAnimationData.RunParameterHash);
        CameraController.Instance.ZoomInToDefault();
        CameraController.Instance.isRunning = false;
    }
    
}
