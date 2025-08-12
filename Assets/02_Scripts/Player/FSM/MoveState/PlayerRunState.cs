using UnityEngine;

public class PlayerRunState : PlayerMoveState
{
    private CameraTargetMover _cameraTargetMover;
    private float _staminaConsumeTimer;
    private PlayerCameraController _playerCameraController;
    public PlayerRunState(PlayerStateMachine playerStateMachine) : base(playerStateMachine)
    {
    }

    public override void Enter()
    {
        base.Enter();
        _staminaConsumeTimer = 0f;
        if (_cameraTargetMover == null)
        {
            _cameraTargetMover = stateMachine.Player.transform.Find("CameraTarget").GetComponent<CameraTargetMover>();
        }

        _cameraTargetMover.MoveToOffset(new UnityEngine.Vector3(0f, 0.7f, 0f), 0.5f); // 예시: 위로 0.7만큼 0.5초 동안
        stateMachine.MovementSpeedModifier = moveData.RunSpeedModifier;
        StartAnimation(stateMachine.Player.PlayerAnimationData.RunParameterHash);
        if (_playerCameraController == null)
        {
            GameObject playerCamera = GameObject.FindGameObjectWithTag("PlayerCamera");
            _playerCameraController = playerCamera.GetComponent<PlayerCameraController>();
        }
        _playerCameraController.ZoomOutForRunning();
        _playerCameraController.isRunning = true;
    }

    public override void Update()
    {
        base.Update();
        _staminaConsumeTimer += Time.deltaTime;
        if (_staminaConsumeTimer >= 1f)
        {
            stateMachine.Player.PlayerStat.Consume(StatType.Stamina, 5);
            _staminaConsumeTimer -= 1f; // 타이머에서 1초를 빼서 다음 소모를 준비
        }
    }
    
    public override void Exit()
    {
        base.Exit();
        _cameraTargetMover.ResetPosition(0.3f);
        EndAnimation(stateMachine.Player.PlayerAnimationData.RunParameterHash);
        _playerCameraController.ZoomInToDefault();
        _playerCameraController.isRunning = false;
    }
    
}
