using UnityEngine;

public class PlayerSwordAttackState : PlayerActionState
{
    private readonly SwordAttack _hitBox;
    public PlayerSwordAttackState(PlayerStateMachine playerStateMachine) : base(playerStateMachine)
    {
        _hitBox = stateMachine.Player.GetComponentInChildren<SwordAttack>();
    }

    public override void Enter()
    {
        base.Enter();
        stateMachine.Player.WeaponHandler.OnSwordAttackPoint();
        _hitBox.OnTriggered += stateMachine.Player.WeaponHandler.SwordAttack;
        StartAnimation(stateMachine.Player.PlayerAnimationData.SwordAttackParameterHash);
        SoundManager.Instance.PlayRandomAttackSFX();
    }
    
    public override void Exit()
    {
        base.Exit();
        stateMachine.Player.WeaponHandler.OffSwordAttackPoint();
        _hitBox.OnTriggered -= stateMachine.Player.WeaponHandler.SwordAttack;
        EndAnimation(stateMachine.Player.PlayerAnimationData.SwordAttackParameterHash);
    }
}
