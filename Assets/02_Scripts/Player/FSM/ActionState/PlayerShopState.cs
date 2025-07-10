using UnityEngine;

public class PlayerShopState : PlayerActionState
{
    public PlayerShopState(PlayerStateMachine playerStateMachine) : base(playerStateMachine) { }

    public override void Enter()
    {
        Debug.Log("--- 상점 상태 진입 ---");
        base.Enter();
        stateMachine.MovementSpeedModifier = 0f;
    }

    public override void Update()
    {
    }
    
    public override void PhysicsUpdate()
    {
    }

    public override void Exit()
    {
        Debug.Log("--- 상점 상태 퇴장 ---");
        base.Exit();
        stateMachine.MovementSpeedModifier = 1f;
    }
}