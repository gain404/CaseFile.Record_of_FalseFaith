using UnityEngine;

public class PlayerTalismanAttackState : PlayerActionState
{
    public PlayerTalismanAttackState(PlayerStateMachine playerStateMachine) : base(playerStateMachine)
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
        stateMachine.Player.WeaponHandler.TalismanAttack();
    }
    
    public override void Exit()
    {
        base.Exit();
        EndAnimation(stateMachine.Player.PlayerAnimationData.TalismanAttackParameterHash);
        PoolManager.Instance.Return(PoolKey.PlayerAmuletProjectile, GameObject.FindGameObjectWithTag("Player"));
    }
}
