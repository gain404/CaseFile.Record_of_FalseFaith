using UnityEngine;

public class PlayerGroundState : PlayerBaseState
{
    public PlayerGroundState(PlayerStateMachine playerStateMachine) : base(playerStateMachine)
    {
    }
    
    public override void Enter() //진입할 때
    {
        base.Enter();
        StartAnimation(stateMachine.Player.PlayerAnimationData.GroundParameterHash);
    }
    
    public override void Exit()
    {
        base.Exit();
        EndAnimation(stateMachine.Player.PlayerAnimationData.GroundParameterHash);
    }
    
}
