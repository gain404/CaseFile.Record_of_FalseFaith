using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMoveState : PlayerBaseState
{
    public PlayerMoveState(PlayerStateMachine playerStateMachine) : base(playerStateMachine)
    {
    }

    public override void Enter() //진입할 때
    {
        base.Enter();
        stateMachine.MovementSpeedModifier = 1f;
        StartAnimation(stateMachine.Player.PlayerAnimationData.MoveParameterHash);
    }

    public override void Update()
    {
        
    }

    public override void Exit()
    {
        base.Exit();
        stateMachine.MovementSpeedModifier = 0f;
        EndAnimation(stateMachine.Player.PlayerAnimationData.MoveParameterHash);
    }

    protected override void OnMoveCanceled(InputAction.CallbackContext context)
    {
        if (stateMachine.MovementInput == Vector2.zero)
            return;
        
        stateMachine.ChangeState(stateMachine.IdleState);

        base.OnMoveCanceled(context);
    }
    
}
