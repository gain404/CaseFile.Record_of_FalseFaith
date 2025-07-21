using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMoveState : PlayerBaseState
{
    public float timeSinceNoInput;
    public float gracePeriod = 0.1f;
    public PlayerMoveState(PlayerStateMachine playerStateMachine) : base(playerStateMachine)
    {
    }

    public override void Enter() //진입할 때
    {
        base.Enter();
        timeSinceNoInput = 0f;
        StartAnimation(stateMachine.Player.PlayerAnimationData.MoveParameterHash);
    }

    public override void Update()
    {
        if (Mathf.Abs(stateMachine.MovementInput.x) < 0.01f)
        {
            // 입력이 없으면 타이머 시간을 증가시킴
            timeSinceNoInput += Time.deltaTime;
        }
        else
        {
            // 입력이 있으면 타이머를 다시 0으로 리셋
            timeSinceNoInput = 0f;
        }
    }

    public override void Exit()
    {
        base.Exit();
        stateMachine.MovementSpeedModifier = 0f;
        EndAnimation(stateMachine.Player.PlayerAnimationData.MoveParameterHash);
    }
    
}
