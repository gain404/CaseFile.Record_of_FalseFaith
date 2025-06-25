using UnityEngine;

public class PlayerController : MonoBehaviour
{
    //playerInput 스크립트에 있는 playerActions 가져다 쓰기
    public PlayerInput playerInput { get; private set; }
    public PlayerInput.PlayerActions playerActions { get; private set; }

    private void Awake()
    {
        playerInput = new PlayerInput();
        playerActions = playerInput.Player;
    }

    private void OnEnable()
    {
        playerInput.Enable();
    }

    private void OnDisable()
    {
        playerInput.Disable();
    }
}
