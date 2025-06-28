using UnityEngine;

public class PlayerDialogueState : PlayerActionState
{
    
    public PlayerDialogueState(PlayerStateMachine playerStateMachine) : base(playerStateMachine)
    {
    }

    //상태 진입할 때
    public override void Enter()
    {
        base.Enter();
        _rb.linearVelocity = Vector2.zero;
        stateMachine.Player.Animator.SetBool(stateMachine.Player.PlayerAnimationData.MoveParameterHash, false);
        stateMachine.Player.Animator.SetBool(stateMachine.Player.PlayerAnimationData.RunParameterHash, false);
        stateMachine.Player.Animator.SetBool(stateMachine.Player.PlayerAnimationData.WalkParameterHash, false);
        
        StartAnimation(stateMachine.Player.PlayerAnimationData.IdleParameterHash);

        stateMachine.MovementSpeedModifier = 0f;
        
        var npc = stateMachine.Player.CurrentInteractableNPC;
        var item = stateMachine.Player.CurrentInteractableItem;

        if (npc != null)
        {
            DialogueManager.Instance.StartDialogue(npc.npcData.dialogueAsset);
        }
        if (item != null)
        {
            DialogueManager.Instance.StartItemDialogue(item.dialogueData.dialogueLines);
        }
    }

    //상태 빠져나올 때
    public override void Exit()
    {
        base.Exit();
        EndAnimation(stateMachine.Player.PlayerAnimationData.IdleParameterHash);
        stateMachine.MovementSpeedModifier = 1f;
    }
    public override void HandleInput()
    {
        var playerActions = stateMachine.Player.PlayerController.playerActions;
        bool confirm = playerActions.Confirm.WasPressedThisFrame();
        bool click = playerActions.Click.WasPressedThisFrame();

        if (confirm || click)
        {
            DialogueManager.Instance.HandleClick();
        }
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
