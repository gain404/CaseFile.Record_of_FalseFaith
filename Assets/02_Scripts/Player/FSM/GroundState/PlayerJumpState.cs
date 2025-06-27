using UnityEngine;

public class PlayerJumpState : PlayerGroundState
{
    public PlayerJumpState(PlayerStateMachine playerStateMachine) : base(playerStateMachine)
    {
    }

    public override void Enter()
    {
        stateMachine.JumpForce = groundData.JumpForce;
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
        _rb.AddForce(Vector2.up * stateMachine.JumpForce, ForceMode2D.Impulse);
    }
    
}
