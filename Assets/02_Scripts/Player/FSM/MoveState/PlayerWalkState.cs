
using UnityEngine;

public class PlayerWalkState : PlayerMoveState
{
    private float staminaRegenTimer = 0f;
    public PlayerWalkState(PlayerStateMachine playerStateMachine) : base(playerStateMachine)
    {
    }

    public override void Enter()
    {
        stateMachine.MovementSpeedModifier = moveData.WalkSpeedModifier;
        base.Enter();
        StartAnimation(stateMachine.Player.PlayerAnimationData.WalkParameterHash);
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
        EndAnimation(stateMachine.Player.PlayerAnimationData.WalkParameterHash);
    }
}
