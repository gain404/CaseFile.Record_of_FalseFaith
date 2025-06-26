using UnityEngine;

public class PlayerStateMachine : StateMachine
{
    public Player Player { get; }
    
    //State
    public PlayerIdleState IdleState { get; }
    public PlayerWalkState WalkState { get; }
    public PlayerRunState RunState { get; }
    public PlayerJumpState JumpState { get; }
    
    //움직임 보정값
    public Vector2 MovementInput { get; set; }
    public float MovementSpeed { get; set; }
    public float MovementSpeedModifier { get; set; } = 1f;
    
    
    public float JumpForce { get; set; }
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
        
        //---------------상태 Change------------//
   
        AddTransition(new StateTransition(
            IdleState, RunState,
            ()=> MovementInput != Vector2.zero 
                 && Player.PlayerController.playerActions.Run.ReadValue<float>() > 0.5f));
        
        AddTransition(new StateTransition(
            IdleState, WalkState,
            () => MovementInput != Vector2.zero));
        
        AddTransition(new StateTransition(
            RunState, IdleState,
            ()=> MovementInput == Vector2.zero));
        
        AddTransition(new StateTransition(
            WalkState, IdleState,
            ()=> MovementInput == Vector2.zero));
        
        AddTransition(new StateTransition(
            WalkState, RunState,
            ()=> MovementInput != Vector2.zero 
                 &&Player.PlayerController.playerActions.Run.ReadValue<float>() > 0.5f));
        
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
