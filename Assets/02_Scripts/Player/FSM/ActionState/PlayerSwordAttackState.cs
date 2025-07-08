using UnityEngine;

public class PlayerSwordAttackState : PlayerActionState
{
    public PlayerSwordAttackState(PlayerStateMachine playerStateMachine) : base(playerStateMachine)
    {
    }

    public override void Enter()
    {
        base.Enter();
        Debug.Log("SwordAttackState 진입");
        StartAnimation(stateMachine.Player.PlayerAnimationData.SwordAttackParameterHash);
        stateMachine.Player.WeaponHandler.SwordAttack();
    }
    
    public override void Exit()
    {
        base.Exit();
        EndAnimation(stateMachine.Player.PlayerAnimationData.SwordAttackParameterHash);
        stateMachine.Player.WeaponHandler.StopSwordAttack();
    }
}
