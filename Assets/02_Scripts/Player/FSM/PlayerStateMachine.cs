using UnityEngine;

public class PlayerStateMachine : StateMachine
{
    public Player Player { get; }
    
    //State
    public PlayerIdleState IdleState { get; }
    public PlayerWalkState WalkState { get; }
    public PlayerRunState RunState { get; }
    public PlayerJumpState JumpState { get; }
    public PlayerDashState DashState { get; }
    public PlayerDialogueState DialogueState { get; }
    
    //움직임 보정값
    public Vector2 MovementInput { get; set; }
    public float MovementSpeed { get; set; }
    public float MovementSpeedModifier { get; set; } = 1f;


    public float JumpForce { get; set; } = 3f;
    public float DashForce { get; set; } = 5f;
    public Transform MainCameraTransform { get; set; }

    public PlayerStateMachine(Player player)
    {
        this.Player = player;
        
        MainCameraTransform = Camera.main.transform;
        
        MovementInput = player.PlayerController.playerActions.Move.ReadValue<Vector2>();
        MovementSpeed = player.Data.MoveData.Speed;
        
        //-------------각 상태 초기화------------//
        IdleState = new PlayerIdleState(this);
        WalkState = new PlayerWalkState(this);
        RunState = new PlayerRunState(this);
        JumpState = new PlayerJumpState(this);
        DashState = new PlayerDashState(this);
        DialogueState = new PlayerDialogueState(this);
        
        //---------------상태 Change------------//
        
        //Jump
        AddTransition(new StateTransition(
            IdleState, JumpState,
            () => Player.PlayerController.playerActions.Jump.ReadValue<float>() > 0.5f));
        
        AddTransition(new StateTransition(
            WalkState, JumpState,
            () => Player.PlayerController.playerActions.Jump.ReadValue<float>() > 0.5f));
        
        AddTransition(new StateTransition(
            RunState, JumpState,
            () => Player.PlayerController.playerActions.Jump.ReadValue<float>() > 0.5f));
        
        //Dash
        AddTransition(new StateTransition(
            IdleState, DashState,
            ()=> Mathf.Abs(MovementInput.x) > 0.01f 
                 &&Player.PlayerController.playerActions.Dash.ReadValue<float>() > 0.5f));
        
        AddTransition(new StateTransition(
            WalkState, DashState,
            ()=> Mathf.Abs(MovementInput.x) > 0.01f 
                 &&Player.PlayerController.playerActions.Dash.ReadValue<float>() > 0.5f));
        
        AddTransition(new StateTransition(
            RunState, DashState,
            ()=> Mathf.Abs(MovementInput.x) > 0.01f 
                 &&Player.PlayerController.playerActions.Dash.ReadValue<float>() > 0.5f));
        
        AddTransition(new StateTransition(
            JumpState, DashState,
            ()=> Mathf.Abs(MovementInput.x) > 0.01f 
                 &&Player.PlayerController.playerActions.Dash.ReadValue<float>() > 0.5f));
        
        //Walk
        AddTransition(new StateTransition(
            IdleState, WalkState,
            () => Mathf.Abs(MovementInput.x) > 0.01f
                  && Player.PlayerController.playerActions.Jump.ReadValue<float>() <= 0f));
        
        //Run
        AddTransition(new StateTransition(
            IdleState, RunState,
            ()=> Mathf.Abs(MovementInput.x) > 0.01f
                 && Player.PlayerController.playerActions.Run.ReadValue<float>() > 0.5f));
        
        AddTransition(new StateTransition(
            WalkState, RunState,
            ()=> Mathf.Abs(MovementInput.x) > 0.01f 
                 &&Player.PlayerController.playerActions.Run.ReadValue<float>() > 0.5f));
        
        //Idle
        AddTransition(new StateTransition(
            JumpState, IdleState,
            () => Player.PlayerController.playerActions.Jump.ReadValue<float>() <= 0f));
        
        AddTransition(new StateTransition(
            RunState, IdleState,
            ()=> MovementInput == Vector2.zero));
        
        AddTransition(new StateTransition(
            WalkState, IdleState,
            ()=> MovementInput == Vector2.zero));
        
        AddTransition(new StateTransition(
            DashState, IdleState,
            ()=> Player.PlayerController.playerActions.Dash.ReadValue<float>() <= 0f));
        AddTransition(new StateTransition(
            DialogueState, IdleState,
            ()=> DialogueManager.Instance.IsDialogueFinished));
        
        //Dialogue
        AddTransition(new StateTransition(
            IdleState, DialogueState,
            () => Player.PlayerController.playerActions.Interact.ReadValue<float>() >= 0.5f));

        AddTransition(new StateTransition(
            WalkState, DialogueState,
            () => Player.PlayerController.playerActions.Interact.ReadValue<float>() >= 0.5f));

        AddTransition(new StateTransition(
            RunState, DialogueState,
            () => Player.PlayerController.playerActions.Interact.ReadValue<float>() >= 0.5f));

        
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
    
}
