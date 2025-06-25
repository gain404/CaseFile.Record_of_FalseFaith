using UnityEngine;

public class PlayerBaseState : IState
{
    //플레이어가 공통으로 쓰는 사항들 (키 입력 받아들이기, 애니메이션 등)
    protected PlayerStateMachine stateMachine;
    protected readonly PlayerMoveData moveData;

    public PlayerBaseState(PlayerStateMachine playerStateMachine)
    {
        stateMachine = playerStateMachine;
        moveData = stateMachine.Player.Data.MoveData;
    }
    
    public virtual void Enter()
    {
        
    }

    public virtual void Exit()
    {
        
    }

    public virtual void HandleInput()
    {
        ReadMovementInput();
    }
    
    public virtual void Update()
    {
        Move();
    }

    public virtual void PhysicsUpdate()
    {
        
    }

    //애니메이션 켜고 끄기
    protected void StartAnimaition(int animationHash)
    {
        stateMachine.Player.Animator.SetBool(animationHash,true);
    }

    protected void EndAnimaition(int animationHash)
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
        //방향 계산
        float horizontal = stateMachine.MovementInput.x;
        float vertical = stateMachine.MovementInput.y;
        Vector3 direction = new Vector2(horizontal, vertical).normalized;

        //실제 움직임
        float speed = stateMachine.MovementSpeed * stateMachine.MovementSpeedModifier;
        stateMachine.Player.CharacterController.Move(direction * speed * Time.deltaTime);

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
    
}
