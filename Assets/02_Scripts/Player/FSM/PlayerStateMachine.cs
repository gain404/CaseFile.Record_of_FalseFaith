using Unity.VisualScripting;
using UnityEngine;

public class PlayerStateMachine : StateMachine
{
    public Player Player { get; }
    
    //State
    public PlayerIdleState IdleState { get; }
    public PlayerWalkState WalkState { get; }
    public PlayerRunState RunState { get; }
    
    
    //움직임 보정값
    public Vector2 MovementInput { get; set; }
    public float MovementSpeed { get; set; }
    public float MovementSpeedModifier { get; set; } = 1f;
    
    
    public float JumpForce { get; set; }
    public Transform MainCameraTransform { get; set; }

    public PlayerStateMachine(Player player)
    {
        this.Player = player;

        IdleState = new PlayerIdleState(this);
        WalkState = new PlayerWalkState(this);
        RunState = new PlayerRunState(this);
        
        MainCameraTransform = Camera.main.transform;
        
        MovementInput = player.PlayerController.playerActions.Move.ReadValue<Vector2>();
        MovementSpeed = player.Data.MoveData.Speed;
    }
    
}
