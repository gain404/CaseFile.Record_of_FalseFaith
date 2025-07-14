using Unity.Cinemachine;
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
        Debug.Log("JumpState 진입");
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
    
    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();
        
        Vector2 velocity = _rb.linearVelocity;
        velocity.x = stateMachine.MovementInput.x * stateMachine.MovementSpeed;
        _rb.linearVelocity = velocity;
    }
    
}
