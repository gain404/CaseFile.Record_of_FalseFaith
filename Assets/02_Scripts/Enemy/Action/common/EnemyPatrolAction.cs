using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;
using Random = System.Random;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "EnemyPatrol", story: "[Self] patrol [moveSpeed] [PatrolZone1] and [PatrolZone2] before [IsPlayerDetected]", category: "Action", id: "9b102e5cc78988c1fc76f64678126e2b")]
public partial class EnemyPatrolAction : Action
{
    [SerializeReference] public BlackboardVariable<GameObject> Self;
    [SerializeReference] public BlackboardVariable<float> MoveSpeed;
    [SerializeReference] public BlackboardVariable<Transform> PatrolZone1;
    [SerializeReference] public BlackboardVariable<Transform> PatrolZone2;
    [SerializeReference] public BlackboardVariable<bool> IsPlayerDetected;
    private Rigidbody2D _rigidbody2D;
    
    protected override Status OnStart()
    {
        if (_rigidbody2D == null)
        {
            _rigidbody2D = Self.Value.GetComponent<Rigidbody2D>();
        }
        //if문 방향 저장 및 저장된 것 불러오기
        
        Random random = new Random();
        int randomNum = random.Next(0, 2);
        int direction = randomNum == 0 ? 1 : -1;
        if (direction > 0)
        {
            Self.Value.transform.localScale *= -1;
        }
        _rigidbody2D.linearVelocityX = direction * MoveSpeed;
        
        return Status.Running;
    }

    protected override Status OnUpdate()
    {
        float distanceRight = Math.Abs(PatrolZone1.Value.position.x - Self.Value.transform.position.x);
        float distanceLeft = Math.Abs(PatrolZone2.Value.position.x - Self.Value.transform.position.x);
        float distance = distanceRight > distanceLeft ? distanceLeft : distanceRight;
        if (distance <= 6.0f)
        {
            _rigidbody2D.linearVelocityX *= -1;
            Self.Value.transform.localScale *= -1;
        }

        if (IsPlayerDetected.Value)
        {
            return Status.Success;
        }
        return Status.Running;
    }
}

