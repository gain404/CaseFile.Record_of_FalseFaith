using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerBaseState : IState
{
    //플레이어가 공통으로 쓰는 사항들 (키 입력 받아들이기, 애니메이션 등)
    protected PlayerStateMachine stateMachine;
    protected readonly PlayerMoveData moveData;
    protected Rigidbody2D _rb;

    public PlayerBaseState(PlayerStateMachine playerStateMachine)
    {
        stateMachine = playerStateMachine;
        moveData = stateMachine.Player.Data.MoveData;
        
        _rb = stateMachine.Player.GetComponent<Rigidbody2D>();
    }
    
    public virtual void Enter()
    {
        AddInputActionsCallback();
    }

    public virtual void Exit()
    {
        RemoveInputActionsCallback();
    }

    public virtual void HandleInput()
    {
        ReadMovementInput();
    }
    
    public virtual void Update()
    {
        HandleInput();
    }

    public virtual void PhysicsUpdate()
    {
        Move();
    }

    //애니메이션 켜고 끄기
    protected void StartAnimation(int animationHash)
    {
        stateMachine.Player.Animator.SetBool(animationHash,true);
    }

    protected void EndAnimation(int animationHash)
    {
        stateMachine.Player.Animator.SetBool(animationHash,false);
    }

    //움직임 읽기
    private void ReadMovementInput()
    {
        stateMachine.MovementInput = stateMachine.Player.PlayerController.playerActions.Move.ReadValue<Vector2>();
    }

    private void Move()
    {
        float horizontal = stateMachine.MovementInput.x;
        float speed = stateMachine.MovementSpeed * stateMachine.MovementSpeedModifier;

        _rb.linearVelocity = new Vector2(horizontal * speed, _rb.linearVelocity.y);

        //방향 전환
        if (Mathf.Abs(horizontal) > 0.01f)
        {
            Rotate(horizontal < 0f);
        }
    }

    private void Rotate(bool isFlip)
    {
        stateMachine.Player.PlayerSpriteRenderer.flipX = isFlip;
    }
    
    protected virtual void AddInputActionsCallback()
    {
        PlayerController input = stateMachine.Player.PlayerController;
        input.playerActions.Move.canceled += OnMoveCanceled;
        input.playerActions.Run.started += OnRunStarted;
        input.playerActions.Interact.started += OnItemInteractStarted;
    }
    
    protected virtual void RemoveInputActionsCallback()
    {
        PlayerController input = stateMachine.Player.PlayerController;
        input.playerActions.Move.canceled -= OnMoveCanceled;
        input.playerActions.Run.started -= OnRunStarted;
        input.playerActions.Interact.started -= OnItemInteractStarted;
    }

    protected virtual void OnRunStarted(InputAction.CallbackContext context)
    {
        
    }

    protected virtual void OnMoveCanceled(InputAction.CallbackContext context)
    {
        
    }
    protected virtual void OnItemInteractStarted(InputAction.CallbackContext context)
    {
        var npc = stateMachine.Player.CurrentInteractableNPC;
        if (npc != null)
        {
            npc.OnInteract();  //대화 시작
        }
    }

    
}
