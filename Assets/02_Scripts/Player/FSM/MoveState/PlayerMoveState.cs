using UnityEngine;

public class PlayerMoveState : PlayerBaseState
{
    public PlayerMoveState(PlayerStateMachine playerStateMachine) : base(playerStateMachine)
    {
    }

    public override void Enter() //진입할 때
    {
        stateMachine.MovementSpeedModifier = 1f;
    }

    public override void Update()
    {
        base.Update();

        float input = stateMachine.MovementInput.magnitude;
    }

    public override void Exit()
    {
        stateMachine.MovementSpeedModifier = 0f;
    }
}
