using UnityEngine;

public class PlayerRunState : PlayerMoveState
{
    private CameraTargetMover cameraTargetMover;
    private float staminaConsumeTimer;
    public PlayerRunState(PlayerStateMachine playerStateMachine) : base(playerStateMachine)
    {
    }

    public override void Enter()
    {
        base.Enter();
        staminaConsumeTimer = 0f;
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

    public override void Update()
    {
        base.Update();
        staminaConsumeTimer += Time.deltaTime;
        if (staminaConsumeTimer >= 1f)
        {
            stateMachine.Player.PlayerStat.Consume(StatType.Stamina, 5);
            staminaConsumeTimer -= 1f; // 타이머에서 1초를 빼서 다음 소모를 준비
        }
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
