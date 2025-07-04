using UnityEngine;

public class PlayerGunAttackState : PlayerActionState
{
    public PlayerGunAttackState(PlayerStateMachine playerStateMachine) : base(playerStateMachine)
    {
    }

    public override void Enter()
    {
        base.Enter();
        StartAnimation(stateMachine.Player.PlayerAnimationData.TalismanAttackParameterHash);
    }

    public override void Update()
    {
        base.Update();
        stateMachine.Player.WeaponHandler.GunAttack();
    }
    
    public override void Exit()
    {
        base.Exit();
        EndAnimation(stateMachine.Player.PlayerAnimationData.TalismanAttackParameterHash);
        stateMachine.Player.WeaponHandler.StopGunAttack();
    }
}
