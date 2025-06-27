using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "Jump", story: "[Self] Jump And Plus [JumpCount]", category: "Action", id: "73c1b64e0513aebb6ab8cddb0d8a03f1")]
public partial class JumpAction : Action
{
    [SerializeReference] public BlackboardVariable<GameObject> Self;
    [SerializeReference] public BlackboardVariable<int> JumpCount;

    public float jumpForceY = 10f;
    public float jumpForceX = 5f;
    public float wallCheckDistance = 0.5f;
    public LayerMask wallLayer;
    public LayerMask groundLayer;
    public Vector2 groundCheckOffset = new Vector2(0, -0.5f);
    public float groundCheckRadius = 0.1f;

    private Rigidbody2D bossRigidbody2D;
    private Transform bossTransform;
    private bool hasJumped = false;
    private int horizontalDirection = 1;
    
    protected override Status OnStart()
    {
        GameObject bossGameObject = Self.Value;
        if (bossGameObject == null)
        {
            return Status.Failure;
        }

        bossRigidbody2D = bossGameObject.GetComponent<Rigidbody2D>();
        bossTransform = bossGameObject.transform;

        if (bossRigidbody2D == null)
        {
            return Status.Failure;
        }

        // 좌우 랜덤 방향 설정 (-1: 왼쪽, 1: 오른쪽)
        horizontalDirection = UnityEngine.Random.value < 0.5f ? -1 : 1;

        // 전방 Raycast로 벽 체크
        Vector2 raycastOrigin = bossTransform.position;
        Vector2 raycastDirection = Vector2.right * horizontalDirection;
        RaycastHit2D wallHit = Physics2D.Raycast(
            raycastOrigin,
            raycastDirection,
            wallCheckDistance,
            wallLayer
        );

        // 벽이 있으면 방향 반대로 변경
        if (wallHit.collider != null)
        {
            horizontalDirection *= -1;
        }

        // 점프 force 적용
        Vector2 jumpForce = new Vector2(jumpForceX * horizontalDirection, jumpForceY);
        bossRigidbody2D.AddForce(jumpForce, ForceMode2D.Impulse);

        hasJumped = true;
        return Status.Running; // 점프 완료 후 착지 대기
    }

    protected override Status OnUpdate()
    {
        if (!hasJumped || bossRigidbody2D == null || bossTransform == null)
        {
            return Status.Failure;
        }

        // 발 밑 기준으로 착지 감지
        Vector2 groundCheckPosition = (Vector2)bossTransform.position + groundCheckOffset;
        bool isGrounded = Physics2D.OverlapCircle(
            groundCheckPosition,
            groundCheckRadius,
            groundLayer
        );

        // 착지했고 수직 속도가 거의 0이면 점프 완료
        if (isGrounded && Mathf.Abs(bossRigidbody2D.linearVelocity.y) < 0.01f)
        {
            return Status.Success;
        }

        return Status.Running; // 아직 공중이면 대기
    }

    protected override void OnEnd()
    {
    }
}

