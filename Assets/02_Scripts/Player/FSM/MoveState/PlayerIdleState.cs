using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerIdleState : PlayerMoveState
{
    public PlayerIdleState(PlayerStateMachine playerStateMachine) : base(playerStateMachine)
    {
    }

    public override void Enter()
    {
        stateMachine.MovementSpeedModifier = 0f;
        base.Enter();
        StartAnimation(stateMachine.Player.PlayerAnimationData.IdleParameterHash);
    }

    public override void Exit()
    {
        base.Exit();
        EndAnimation(stateMachine.Player.PlayerAnimationData.IdleParameterHash);
    }

    public override void Update()
    {
        base.Update();

        if (stateMachine.MovementInput != Vector2.zero)
        {
            if (stateMachine.Player.PlayerController.playerActions.Run.ReadValue<float>() > 0.5f)
            {
                stateMachine.ChangeState(stateMachine.RunState);
            }
            else
            {
                stateMachine.ChangeState(stateMachine.WalkState);
            }
        }
    }
}
