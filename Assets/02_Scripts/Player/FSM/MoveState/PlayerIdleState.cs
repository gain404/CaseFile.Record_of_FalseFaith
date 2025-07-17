

using UnityEngine;

public class PlayerIdleState : PlayerMoveState
{
    private float staminaRegenTimer = 0f;
    public PlayerIdleState(PlayerStateMachine playerStateMachine) : base(playerStateMachine)
    {
    }

    public override void Enter()
    {
        stateMachine.MovementSpeedModifier = 0f;
        base.Enter();
        StartAnimation(stateMachine.Player.PlayerAnimationData.IdleParameterHash);
    }
    
    public override void Update()
    {
        base.Update();
    
        staminaRegenTimer += Time.deltaTime;
        if (staminaRegenTimer >= 1f)
        {
            // Recover를 직접 호출
            stateMachine.Player.PlayerStat.Recover(StatType.Stamina, 5);
            staminaRegenTimer -= 1f;
        }
    }

    public override void Exit()
    {
        base.Exit();
        EndAnimation(stateMachine.Player.PlayerAnimationData.IdleParameterHash);
    }
}
