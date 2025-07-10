using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;
using Random = System.Random;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "EnemyPatrol", story: "[Self] patrol to [Direction] [moveSpeed] [PatrolZone1] and [PatrolZone2] before [IsPlayerDetected]", category: "Action", id: "9b102e5cc78988c1fc76f64678126e2b")]
public partial class EnemyPatrolAction : Action
{
    [SerializeReference] public BlackboardVariable<GameObject> Self;
    [SerializeReference] public BlackboardVariable<int> Direction;
    [SerializeReference] public BlackboardVariable<float> MoveSpeed;
    [SerializeReference] public BlackboardVariable<float> PatrolZone1;
    [SerializeReference] public BlackboardVariable<float> PatrolZone2;
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
        Direction.Value = randomNum == 0 ? 1 : -1;
        if (Direction.Value > 0)
        {
            var scale = Self.Value.transform.localScale;
            scale.x = scale.x * -1;
            Self.Value.transform.localScale = scale;
        }

        _rigidbody2D.linearVelocityX = Direction.Value * MoveSpeed.Value;
        
        return Status.Running;
    }

    protected override Status OnUpdate()
    {
        float distanceRight = Math.Abs(PatrolZone1.Value - Self.Value.transform.position.x);
        float distanceLeft = Math.Abs(PatrolZone2.Value - Self.Value.transform.position.x);
        float distance = distanceRight > distanceLeft ? distanceLeft : distanceRight;
        if (distance <= 6.0f)
        {
            _rigidbody2D.linearVelocityX *= -1;
            var scale = Self.Value.transform.localScale;
            scale.x = scale.x * -1;
            Self.Value.transform.localScale = scale;
        }

        if (IsPlayerDetected.Value)
        {
            return Status.Success;
        }
        return Status.Running;
    }
}

