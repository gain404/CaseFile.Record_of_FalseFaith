using UnityEngine;

public class PlayerRunState : PlayerMoveState
{
    public PlayerRunState(PlayerStateMachine playerStateMachine) : base(playerStateMachine)
    {
    }

    public override void Enter()
    {
        base.Enter();
        StartAnimation(stateMachine.Player.PlayerAnimationData.RunParameterHash);
    }

    public override void Exit()
    {
        base.Exit();
        EndAnimation(stateMachine.Player.PlayerAnimationData.RunParameterHash);
    }
}
