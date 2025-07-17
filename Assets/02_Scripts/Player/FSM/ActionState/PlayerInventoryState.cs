using UnityEngine;

public class PlayerInventoryState : PlayerActionState
{
    public PlayerInventoryState(PlayerStateMachine playerStateMachine) : base(playerStateMachine)
    {

    }

    public override void Enter()
    {
        base.Enter();
        _rb.linearVelocity = Vector2.zero;
        stateMachine.Player.Animator.SetBool(stateMachine.Player.PlayerAnimationData.MoveParameterHash, false);
        stateMachine.Player.Animator.SetBool(stateMachine.Player.PlayerAnimationData.RunParameterHash, false);
        stateMachine.Player.Animator.SetBool(stateMachine.Player.PlayerAnimationData.WalkParameterHash, false);

        StartAnimation(stateMachine.Player.PlayerAnimationData.IdleParameterHash);

        stateMachine.MovementSpeedModifier = 0f;

        UIManager.Instance.GetUI<UIInventory>(UIType.Inventory).Toggle();
    }

    //상태 빠져나올 때
    public override void Exit()
    {
        UIManager.Instance.GetUI<UIInventory>(UIType.Inventory).Toggle();
        base.Exit();
        EndAnimation(stateMachine.Player.PlayerAnimationData.IdleParameterHash);
        stateMachine.MovementSpeedModifier = 1f;
    }

    public override void Update()
    {
        // HandleInput만 처리함 (움직임/점프 등은 무시)
        HandleInput();
    }

    public override void PhysicsUpdate()
    {
        // 움직임 없음
    }
}
