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
        Debug.Log("SwordAttackState 진입");
        StartAnimation(stateMachine.Player.PlayerAnimationData.SwordAttackParameterHash);
        _hitBox.OnTriggered += stateMachine.Player.WeaponHandler.SwordAttack;
    }
    
    public override void Exit()
    {
        base.Exit();
        stateMachine.Player.WeaponHandler.OffSwordAttackPoint();
        EndAnimation(stateMachine.Player.PlayerAnimationData.SwordAttackParameterHash);
        _hitBox.OnTriggered -= stateMachine.Player.WeaponHandler.SwordAttack;
    }
}
