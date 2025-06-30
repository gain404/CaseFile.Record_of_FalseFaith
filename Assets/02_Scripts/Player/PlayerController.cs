using System;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    //playerInput 스크립트에 있는 playerActions 가져다 쓰기
    public PlayerInput playerInput { get; private set; }
    public PlayerInput.PlayerActions playerActions { get; private set; }

    public bool isGrounded;
    public Action inventory; //추가(송도현)

    [SerializeField] private Transform playerTransform;
    [SerializeField] private float checkRadius;
    [SerializeField] private LayerMask groundLayer;

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

    public void CheckGround()
    {
        isGrounded = Physics2D.OverlapCircle(playerTransform.position, checkRadius, groundLayer) != null;
    }
    
#if UNITY_EDITOR
    private void OnDrawGizmosSelected()
    {
        if (playerTransform == null) return;
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(playerTransform.position, checkRadius);
    }
#endif

}
