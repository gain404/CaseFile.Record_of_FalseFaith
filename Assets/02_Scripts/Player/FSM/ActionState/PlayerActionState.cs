using UnityEngine;

public class PlayerActionState : PlayerBaseState
{
    public PlayerActionState(PlayerStateMachine playerStateMachine) : base(playerStateMachine)
    {
    }

    public override void Enter() //진입할 때
    {
        base.Enter();
        StartAnimation(stateMachine.Player.PlayerAnimationData.ActionParameterHash);
    }

    public override void Update()
    {
        
    }

    public override void Exit()
    {
        base.Exit();
        EndAnimation(stateMachine.Player.PlayerAnimationData.ActionParameterHash);
    }
    
}
