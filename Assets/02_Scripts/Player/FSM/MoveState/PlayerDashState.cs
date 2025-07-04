// PlayerDashState.cs

using UnityEngine;
using System.Collections;

public class PlayerDashState : PlayerMoveState
{
    private readonly float _dashingTime = 0.2f; // 대쉬 지속 시간
    private float _originalGravityScale;

    public PlayerDashState(PlayerStateMachine playerStateMachine) : base(playerStateMachine)
    {
    }

    // PlayerDashState.cs

    public override void Enter()
    {
        base.Enter();
        StartAnimation(stateMachine.Player.PlayerAnimationData.DashParameterHash);

        stateMachine.IsDashFinished = false;

        // 공중에서 대쉬를 시작했는지 확인
        if (!stateMachine.Player.PlayerController.isGrounded)
        {
            // 아직 공중 대쉬를 사용하지 않은 상태였다면 사용했다고 기록하고 로그 출력
            if (!stateMachine.Player.PlayerController.hasAirDashed)
            {
                stateMachine.Player.PlayerController.hasAirDashed = true;
            }
        }

        _originalGravityScale = _rb.gravityScale;
        _rb.gravityScale = 0f;

        float direction = stateMachine.Player.transform.localScale.x < 0f ? -1f : 1f;
        if (stateMachine.MovementInput != Vector2.zero)
        {
            direction = stateMachine.MovementInput.x > 0 ? 1f : -1f;
        }
    
        float dashSpeed = stateMachine.Player.Data.MoveData.DashSpeedModifier;
        _rb.linearVelocity = new Vector2(direction * dashSpeed, 0f);

        stateMachine.Player.StartCoroutine(DashEndCoroutine());
    }

    public override void Exit()
    {
        base.Exit();
        // 6. 대쉬 상태를 빠져나갈 때, 중력을 원래대로 복구
        _rb.gravityScale = _originalGravityScale;
        EndAnimation(stateMachine.Player.PlayerAnimationData.DashParameterHash);
    }

    public override void PhysicsUpdate()
    {
        // 대쉬 중에는 플레이어의 추가적인 물리 업데이트를 막습니다.
    }

    private IEnumerator DashEndCoroutine()
    {
        // 정해진 시간만큼 기다립니다.
        yield return new WaitForSeconds(_dashingTime);
        // 시간이 지나면 대쉬가 끝났다고 알려줍니다.
        stateMachine.IsDashFinished = true;
    }
}