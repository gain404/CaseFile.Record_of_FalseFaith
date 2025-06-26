using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class TestPlayerController : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed;
    private Vector2 movementInput;  // 현재 입력 값
    public float jumpPower;
    public LayerMask groundLayerMask;  // 레이어 정보

    [HideInInspector]
    public bool canJump = true;

    private Rigidbody2D rigidbody;

    public Action inventory;

    private void Awake()
    {
        rigidbody = GetComponent<Rigidbody2D>();
    }

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }

    private void Update()
    {
        // 수평 이동 처리
        rigidbody.linearVelocity = new Vector2(movementInput.x * moveSpeed, rigidbody.linearVelocity.y);

        // 점프 처리
        if (canJump && IsGrounded())
        {
            rigidbody.linearVelocity = new Vector2(rigidbody.linearVelocity.x, jumpPower);
            canJump = false; // 중복 점프 방지
        }
    }


    // 입력값 처리
    public void OnMoveInput(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Performed)
        {
            movementInput = context.ReadValue<Vector2>();
        }
        else if (context.phase == InputActionPhase.Canceled)
        {
            movementInput = Vector2.zero;
        }
    }

    public void OnJumpInput(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Started && IsGrounded())
        {
            rigidbody.AddForce(Vector2.up * jumpPower, ForceMode2D.Impulse);
        }
    }

    public void OnInventoryInput(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Started)
        {
            inventory?.Invoke();
            ToggleCursor();
        }
    }

    void ToggleCursor()
    {
        bool toggle = Cursor.lockState == CursorLockMode.Locked;
        Cursor.lockState = toggle ? CursorLockMode.None : CursorLockMode.Locked;
    }


    bool IsGrounded()
    {
        // 4개의 Ray를 만든다.
        // 플레이어(transform)을 기준으로 앞뒤좌우 0.2씩 떨어뜨려서.
        // 0.01 정도 살짝 위로 올린다.
        // 하이라이트 부분의 차이점과 그 외 부분을 나눠서 분석해보세요.
        Ray[] rays = new Ray[4]
        {
            new Ray(transform.position + (transform.forward * 0.2f) + (transform.up * 0.01f), Vector3.down),
            new Ray(transform.position + (-transform.forward * 0.2f) + (transform.up * 0.01f), Vector3.down),
            new Ray(transform.position + (transform.right * 0.2f) + (transform.up * 0.01f), Vector3.down),
            new Ray(transform.position + (-transform.right * 0.2f) +(transform.up * 0.01f), Vector3.down)
        };

        // 4개의 Ray 중 groundLayerMask에 해당하는 오브젝트가 충돌했는지 조회한다.
        for (int i = 0; i < rays.Length; i++)
        {
            if (Physics.Raycast(rays[i], 0.1f, groundLayerMask))
            {
                return true;
            }
        }

        return false;
    }
}
