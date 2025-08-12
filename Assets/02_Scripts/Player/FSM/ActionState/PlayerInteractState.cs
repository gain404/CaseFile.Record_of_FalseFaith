using UnityEngine;

public class PlayerInteractState : PlayerActionState
{
    public PlayerInteractState(PlayerStateMachine playerStateMachine) : base(playerStateMachine)
    {
    }

    public override void Enter()
    {
        base.Enter();
        Debug.Log("PlayerInteractState Enter");

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
            DialogueAsset dialogueToStart = null;

            // 🔹 조사 성공 시 SecondDialogue 출력
            if (stateMachine.IsReturnFromInvestigationSuccess)
            {
                dialogueToStart = npc.GetSecondDialogue();
                stateMachine.IsReturnFromInvestigationSuccess = false; // 플래그 초기화
            }

            if (dialogueToStart == null)
            {
                dialogueToStart = npc.GetFirstDialogue();
            }

            if (dialogueToStart != null)
            {
                UIManager.Instance.UIDialogue.StartDialogue(dialogueToStart, npc.transform);
            }
            else
            {
                stateMachine.ChangeState(stateMachine.IdleState);
            }
        }
        else if (item != null)
        {
            item.OnInteract();
        }
        else if (itemData != null)
        {
            Interaction interaction = stateMachine.Player.GetComponent<Interaction>();
            if (interaction != null)
            {
                interaction.currentInteractable.OnInteract();
            }
        }
    }

    public override void Exit()
    {
        base.Exit();
        Debug.Log("PlayerInteractState Exit");
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
            UIManager.Instance.UIDialogue.HandleClick();
        }
    }

    public override void Update()
    {
        HandleInput();
    }

    public override void PhysicsUpdate()
    {
        // 움직임 없음
    }
}
