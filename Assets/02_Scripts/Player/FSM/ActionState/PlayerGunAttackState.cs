using UnityEngine;

public class PlayerGunAttackState : PlayerActionState
{
    public PlayerGunAttackState(PlayerStateMachine playerStateMachine) : base(playerStateMachine)
    {
    }

    public override void Enter()
    {
        base.Enter();
        Debug.Log("GunAttackState 진입");
        StartAnimation(stateMachine.Player.PlayerAnimationData.GunAttackParameterHash);
        stateMachine.Player.WeaponHandler.GunAttack();
    }
    
    public override void Exit()
    {
        base.Exit();
        EndAnimation(stateMachine.Player.PlayerAnimationData.GunAttackParameterHash);
    }
}
