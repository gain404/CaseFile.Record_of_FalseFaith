using UnityEngine;

public class PlayerJumpState : PlayerGroundState
{
    public PlayerJumpState(PlayerStateMachine playerStateMachine) : base(playerStateMachine)
    {
    }

    public override void Enter()
    {
        stateMachine.MovementSpeedModifier = moveData.WalkSpeedModifier;
        base.Enter();
        OnJumped();
        StartAnimation(stateMachine.Player.PlayerAnimationData.JumpParameterHash);
    }

    public override void Exit()
    {
        base.Exit();
        EndAnimation(stateMachine.Player.PlayerAnimationData.JumpParameterHash);
    }

    private void OnJumped()
    {
        _rb.AddForce(Vector2.up * stateMachine.Player.Data.GroundData.JumpForce, ForceMode2D.Impulse);
    }
    
}
