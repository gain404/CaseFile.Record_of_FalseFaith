

using UnityEngine;

public class PlayerIdleState : PlayerMoveState
{
    private float _staminaRegenTimer = 0f;
    public PlayerIdleState(PlayerStateMachine playerStateMachine) : base(playerStateMachine)
    {
    }

    public override void Enter()
    {
        Debug.Log("Idle State Enter");
        stateMachine.MovementSpeedModifier = 0f;
        base.Enter();
        StartAnimation(stateMachine.Player.PlayerAnimationData.IdleParameterHash);
    }
    
    public override void Update()
    {
        base.Update();
    
        _staminaRegenTimer += Time.deltaTime;
        if (_staminaRegenTimer >= 1f)
        {
            // Recover를 직접 호출
            stateMachine.Player.PlayerStat.Recover(StatType.Stamina, 5);
            _staminaRegenTimer -= 1f;
        }
    }

    public override void Exit()
    {
        Debug.Log("Idle State Exit");
        base.Exit();
        EndAnimation(stateMachine.Player.PlayerAnimationData.IdleParameterHash);
    }
}
