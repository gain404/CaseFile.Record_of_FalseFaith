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

    protected virtual void AddInputActionsCallback()
    {
        PlayerController input = stateMachine.Player.PlayerController;
        input.playerActions.Move.canceled += OnMoveCanceled;
        input.playerActions.Run.started += OnRunStarted;
    }
    
    protected virtual void RemoveInputActionsCallback()
    {
        PlayerController input = stateMachine.Player.PlayerController;
        input.playerActions.Move.canceled -= OnMoveCanceled;
        input.playerActions.Run.started -= OnRunStarted;
    }

    private void OnRunStarted(InputAction.CallbackContext context)
    {
        
    }

    private void OnMoveCanceled(InputAction.CallbackContext context)
    {
        
    }
}
