using UnityEngine;

public class PlayerStateMachine : StateMachine
{
    public Player Player { get; }
    
    //State
    public PlayerBaseState BaseState { get; }
    public PlayerIdleState IdleState { get; }
    public PlayerWalkState WalkState { get; }
    public PlayerRunState RunState { get; }
    public PlayerJumpState JumpState { get; }
    public PlayerDashState DashState { get; }
    public PlayerInteractState InteractState { get; }
    public PlayerInventoryState InventoryState { get; }
    public PlayerSwordAttackState SwordAttackState { get; }
    public PlayerGunAttackState GunAttackState { get; }
    public PlayerShopState ShopState { get; private set; }
    
    public IState PreviousState { get; private set; }
    //움직임 보정값
    public Vector2 MovementInput { get; set; }
    public float MovementSpeed { get; set; }
    public float MovementSpeedModifier { get; set; } = 1f;


    public float JumpForce { get; set; } = 3f;
    public float DashForce { get; set; } = 5f;
    public bool IsDashFinished { get; set; }
    public Transform MainCameraTransform { get; set; }

    public PlayerStateMachine(Player player)
    {
        this.Player = player;
        
        MainCameraTransform = Camera.main.transform;
        
        MovementInput = player.PlayerController.playerActions.Move.ReadValue<Vector2>();
        MovementSpeed = player.Data.MoveData.Speed;
        
        //-------------각 상태 초기화------------//
        BaseState = new PlayerBaseState(this);
        IdleState = new PlayerIdleState(this);
        WalkState = new PlayerWalkState(this);
        RunState = new PlayerRunState(this);
        JumpState = new PlayerJumpState(this);
        DashState = new PlayerDashState(this);
        InteractState = new PlayerInteractState(this);
        InventoryState = new PlayerInventoryState(this);
        SwordAttackState = new PlayerSwordAttackState(this);
        GunAttackState = new PlayerGunAttackState(this);
        ShopState = new PlayerShopState(this);
        
        //---------------상태 Change------------//
        
        //Jump
        AddTransition(new StateTransition(
            IdleState, JumpState,
            () => Player.PlayerController.playerActions.Jump.triggered
                  && Player.PlayerController.isGrounded));
        
        AddTransition(new StateTransition(
            WalkState, JumpState,
            () => Player.PlayerController.playerActions.Jump.triggered
                  && Player.PlayerController.isGrounded));
        
        AddTransition(new StateTransition(
            RunState, JumpState,
            () => Player.PlayerController.playerActions.Jump.triggered
                  && Player.PlayerController.isGrounded));
        
        //Dash
        AddTransition(new StateTransition(
            IdleState, DashState,
            ()=> Player.PlayerController.playerActions.Dash.WasPressedThisFrame() 
                 && !Player.PlayerController.hasAirDashed)); // 조건 추가

        AddTransition(new StateTransition(
            WalkState, DashState,
            ()=> Player.PlayerController.playerActions.Dash.WasPressedThisFrame() 
                 && !Player.PlayerController.hasAirDashed)); // 조건 추가

        AddTransition(new StateTransition(
            RunState, DashState,
            ()=> Player.PlayerController.playerActions.Dash.WasPressedThisFrame() 
                 && !Player.PlayerController.hasAirDashed)); // 조건 추가
        
        AddTransition(new StateTransition(
            JumpState, DashState,
            ()=> Player.PlayerController.playerActions.Dash.WasPressedThisFrame() 
                 && !Player.PlayerController.hasAirDashed)); // 조건 추가
        
        //Walk
        AddTransition(new StateTransition(
            IdleState, WalkState,
            () => Player.PlayerController.playerActions.Move.IsPressed()
                  && !Player.PlayerController.playerActions.Run.IsPressed()));
        
        AddTransition(new StateTransition(
            RunState, WalkState,
            ()=> Player.PlayerController.playerActions.Run.ReadValue<float>() < 0.01f));
        
        AddTransition(new StateTransition(
            DashState, WalkState,
            () => IsDashFinished
                  && Mathf.Abs(MovementInput.x) > 0.01f));
        
        //Run
        AddTransition(new StateTransition(
            IdleState, RunState,
            ()=> Mathf.Abs(MovementInput.x) > 0.01f
                 && Player.PlayerController.playerActions.Run.IsPressed()));
        
        AddTransition(new StateTransition(
            WalkState, RunState,
            ()=> Mathf.Abs(MovementInput.x) > 0.01f 
                 &&Player.PlayerController.playerActions.Run.IsPressed()));
        
        AddTransition(new StateTransition(
            DashState, RunState,
            () => IsDashFinished
                  && Mathf.Abs(MovementInput.x) > 0.01f
                  && Player.PlayerController.playerActions.Run.IsPressed()));
        
        //Idle
        AddTransition(new StateTransition(
            JumpState, IdleState,
            () => Player.PlayerController.isGrounded && BaseState._rb.linearVelocity.y <= 0f));
        
        AddTransition(new StateTransition(
            RunState, IdleState,
            ()=> MovementInput == Vector2.zero));

        AddTransition(new StateTransition(
            WalkState, IdleState,
            () => Mathf.Abs(MovementInput.x) < 0.01f));
        
        AddTransition(new StateTransition(
            DashState, IdleState,
            () => IsDashFinished));
        
        AddTransition(new StateTransition(
            InteractState, IdleState,
            ()=> DialogueManager.Instance.IsDialogueFinished || Player.itemData == null && Player.CurrentInteractableNPC == null && player.CurrentInteractableItem == null));

        AddTransition(new StateTransition(
            InventoryState, IdleState,
            () => Player.PlayerController.playerActions.Inventory.WasPressedThisFrame()
                    && TestUIManager.Instance.uiInventory.IsOpen() == true&& !ShopManager.Instance.shopPanel.activeSelf));

        AddTransition(new StateTransition(
            SwordAttackState, IdleState,
            () =>
            {
                var info = Player.Animator.GetCurrentAnimatorStateInfo(0);
                return info.shortNameHash == Player.PlayerAnimationData.SwordAttackParameterHash && info.normalizedTime >= 1f;
            }));

        AddTransition(new StateTransition(
            GunAttackState, IdleState,
            () => Player.PlayerController.playerActions.Attack.IsPressed()));
        

        //Interact
        AddTransition(new StateTransition(
            IdleState, InteractState,
            () => Player.PlayerController.playerActions.Interact.WasPressedThisFrame()
                && (Player.CurrentInteractableNPC != null || Player.CurrentInteractableItem != null || Player.itemData != null)));

        AddTransition(new StateTransition(
            WalkState, InteractState,
            () => Player.PlayerController.playerActions.Interact.WasPressedThisFrame()
                && (Player.CurrentInteractableNPC != null || Player.CurrentInteractableItem != null || Player.itemData != null)));

        AddTransition(new StateTransition(
            RunState, InteractState,
            () => Player.PlayerController.playerActions.Interact.WasPressedThisFrame()
                && (Player.CurrentInteractableNPC != null || Player.CurrentInteractableItem != null || Player.itemData != null)));
        
        AddTransition(new StateTransition(
            JumpState, InteractState,
            () => Player.PlayerController.playerActions.Interact.WasPressedThisFrame()
                  && (Player.CurrentInteractableNPC != null || Player.CurrentInteractableItem != null || Player.itemData != null)));
        
        AddTransition(new StateTransition(
            ShopState, InteractState,
            () => !ShopManager.Instance.shopPanel.activeSelf));

        //Inventory
        AddTransition(new StateTransition(
            IdleState, InventoryState,
            () => Player.PlayerController.playerActions.Inventory.WasPressedThisFrame()
                && TestUIManager.Instance.uiInventory.IsOpen() == false));

        AddTransition(new StateTransition(
            WalkState, InventoryState,
            () => Player.PlayerController.playerActions.Inventory.ReadValue<float>() >= 0.5f
                && TestUIManager.Instance.uiInventory.IsOpen() == false));

        AddTransition(new StateTransition(
            RunState, InventoryState,
            () => Player.PlayerController.playerActions.Inventory.ReadValue<float>() >= 0.5f
                && TestUIManager.Instance.uiInventory.IsOpen() == false));

        AddTransition(new StateTransition(
            JumpState, InventoryState,
            () => Player.PlayerController.playerActions.Inventory.ReadValue<float>() >= 0.5f
                && TestUIManager.Instance.uiInventory.IsOpen() == false));
        AddTransition(new StateTransition(
            ShopState, InventoryState,
            () => Player.PlayerController.playerActions.Inventory.WasPressedThisFrame()));
        
        //SwordAttack
        AddTransition(new StateTransition(
            IdleState, SwordAttackState,
            ()=> Player.PlayerController.playerActions.Attack.triggered
                && Player.WeaponHandler.weaponCount == 0));
        
        AddTransition(new StateTransition(
            WalkState, SwordAttackState,
            ()=> Player.PlayerController.playerActions.Attack.triggered
                 && Player.WeaponHandler.weaponCount == 0));
        
        AddTransition(new StateTransition(
            RunState, SwordAttackState,
            ()=> Player.PlayerController.playerActions.Attack.triggered
                 && Player.WeaponHandler.weaponCount == 0));
        
        AddTransition(new StateTransition(
            JumpState, SwordAttackState,
            ()=> Player.PlayerController.playerActions.Attack.triggered
                 && Player.WeaponHandler.weaponCount == 0));
        
        //GunAttack
        AddTransition(new StateTransition(
            IdleState, GunAttackState,
            ()=> Player.PlayerController.playerActions.Attack.triggered
                 && Player.WeaponHandler.weaponCount > 0));
        
        AddTransition(new StateTransition(
            WalkState, GunAttackState,
            ()=> Player.PlayerController.playerActions.Attack.triggered
                 && Player.WeaponHandler.weaponCount > 0));
        
        AddTransition(new StateTransition(
            RunState, GunAttackState,
            ()=> Player.PlayerController.playerActions.Attack.triggered
                 && Player.WeaponHandler.weaponCount > 0));
        
        AddTransition(new StateTransition(
            JumpState, GunAttackState,
            ()=> Player.PlayerController.playerActions.Attack.triggered
                 && Player.WeaponHandler.weaponCount > 0));
        //Shop
        AddTransition(new StateTransition(
            InteractState, ShopState,
            () => DialogueManager.Instance.CurrentState == DialogueManager.DialogueState.Paused));
        
        AddTransition(new StateTransition(
            InventoryState, ShopState,
            () => Player.PlayerController.playerActions.Inventory.WasPressedThisFrame() &&
                  ShopManager.Instance.shopPanel.activeSelf));
    }

    public override void Update()
    {
        foreach (StateTransition transition in transitions)
        {
            if (currentState == transition.From && transition.Condition())
            {
                ChangeState (transition.To);
                break;
            }
        }

        base.Update();
    }
    public override void ChangeState(IState newState)
    {
        PreviousState = currentState;
        base.ChangeState(newState);
    }
    
}
