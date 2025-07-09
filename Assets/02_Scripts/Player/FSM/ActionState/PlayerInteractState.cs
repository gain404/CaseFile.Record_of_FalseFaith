using UnityEngine;

public class PlayerInteractState : PlayerActionState
{
    
    public PlayerInteractState(PlayerStateMachine playerStateMachine) : base(playerStateMachine)
    {
    }

    //상태 진입할 때
    public override void Enter()
    {
        Debug.Log("다이알로그 상태 진입");
        base.Enter();
        _rb.linearVelocity = Vector2.zero;
        stateMachine.Player.Animator.SetBool(stateMachine.Player.PlayerAnimationData.MoveParameterHash, false);
        stateMachine.Player.Animator.SetBool(stateMachine.Player.PlayerAnimationData.RunParameterHash, false);
        stateMachine.Player.Animator.SetBool(stateMachine.Player.PlayerAnimationData.WalkParameterHash, false);

        StartAnimation(stateMachine.Player.PlayerAnimationData.IdleParameterHash);

        stateMachine.MovementSpeedModifier = 0f;

        var npc = stateMachine.Player.CurrentInteractableNPC;
        var item = stateMachine.Player.CurrentInteractableItem;
        ItemData itemData = stateMachine.Player.itemData;

        if (npc == null)
        {
            stateMachine.ChangeState(stateMachine.IdleState);
            return;
        }
        Debug.Log($"[InteractState] 현재 상호작용 NPC: {npc.name}");
        Debug.Log("[InteractState] 이전 상태를 확인하여 보여줄 대화를 결정합니다...");
        DialogueAsset dialogueToStart = null;
        
        if (stateMachine.PreviousState == stateMachine.ShopState)
        {
            Debug.Log("[InteractState] 조건: 상점에서 복귀함. 두 번째 대화를 시도합니다.");
            dialogueToStart = npc.GetSecondDialogue();
        }
        
        if (dialogueToStart == null)
        {
            Debug.Log("[InteractState] 조건: 처음 말을 걸었거나 두 번째 대화가 없음. 첫 번째 대화를 시도합니다.");
            dialogueToStart = npc.GetFirstDialogue();
        }

        Debug.Log($"[InteractState] 최종 선택된 대화 에셋: {(dialogueToStart != null ? dialogueToStart.name : "없음 (null)")}");
        if (dialogueToStart != null)
        {
            DialogueManager.Instance.StartDialogue(dialogueToStart, npc.transform);
        }
        else
        {
            Debug.LogWarning("[InteractState] 보여줄 대화 에셋이 없어 Idle 상태로 돌아갑니다.");
            stateMachine.ChangeState(stateMachine.IdleState);
        }

    }


    //상태 빠져나올 때
    public override void Exit()
    {
        Debug.Log("다이알로그 상태 퇴장");
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
