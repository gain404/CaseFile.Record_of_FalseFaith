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

        if (npc != null)
        {
            Debug.Log($"--- NPC와 상호작용 시작: {npc.name} ---");
        
            DialogueAsset dialogueToStart = null;
            
            if (stateMachine.PreviousState == stateMachine.ShopState)
            {
                dialogueToStart = npc.GetSecondDialogue();
            }
            
            if (dialogueToStart == null)
            {
                dialogueToStart = npc.GetFirstDialogue();
            }
        
            if (dialogueToStart != null)
            {
                DialogueManager.Instance.StartDialogue(dialogueToStart, npc.transform);
            }
            else
            {
                stateMachine.ChangeState(stateMachine.IdleState);
            }
        }
        else if (item != null)
        {
            Debug.Log($"--- 필드 아이템과 상호작용 시작: {item.name} ---");
            item.OnInteract();
        }
        else if (itemData != null)
        {
            Debug.Log("--- 기타 아이템 데이터 상호작용 시작 ---");
            Interaction interaction = stateMachine.Player.GetComponent<Interaction>();
            if (interaction != null)
            {
                interaction.currentInteractable.OnInteract();
            }
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
