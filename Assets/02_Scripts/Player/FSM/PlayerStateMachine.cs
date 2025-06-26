using UnityEngine;

public class PlayerStateMachine : StateMachine
{
    public Player Player { get; }
    
    public Vector2 MovementInput { get; set; }
    public float MovementSpeed { get; set; }
    public float MovementSpeedModifier { get; set; }
    
    public float JumpForce { get; set; }
    public Transform MainCameraTransform { get; set; }

    public PlayerStateMachine(Player player)
    {
        this.Player = player;
        
        MainCameraTransform = Camera.main.transform;
    }
    
}
