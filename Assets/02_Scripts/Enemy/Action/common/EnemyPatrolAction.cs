using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;
using Random = System.Random;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "EnemyPatrol", story: "[Self] patrol to [Direction] [moveSpeed] for searching [Target] before [IsPlayerDetected] [StartTransform]", category: "Action", id: "9b102e5cc78988c1fc76f64678126e2b")]
public partial class EnemyPatrolAction : Action
{
    [SerializeReference] public BlackboardVariable<GameObject> Self;
    [SerializeReference] public BlackboardVariable<int> Direction;
    [SerializeReference] public BlackboardVariable<float> MoveSpeed;
    [SerializeReference] public BlackboardVariable<GameObject> Target;
    [SerializeReference] public BlackboardVariable<bool> IsPlayerDetected;
    [SerializeReference] public BlackboardVariable<Transform> StartTransform;
    private Rigidbody2D _rigidbody2D;
    private float _distance;
    private int _direction;
    
    protected override Status OnStart()
    {
        if (_rigidbody2D == null)
        {
            _rigidbody2D = Self.Value.GetComponent<Rigidbody2D>();
        }

        StartTransform.Value = Self.Value.transform;
        //if문 방향 저장 및 저장된 것 불러오기

        if (_distance > 150)
        {
            Self.Value.transform.position = StartTransform.Value.position;
            return Status.Success;
        }
        
        Random random = new Random();
        int randomNum = random.Next(0, 2);
        Direction.Value = randomNum == 0 ? 1 : -1;
        if (Direction.Value > 0)
        {
            var scale = Self.Value.transform.localScale;
            scale.x = scale.x * -1;
            Self.Value.transform.localScale = scale;
        }
        ChangeDirection();
        
        return Status.Running;
    }

    protected override Status OnUpdate()
    {
        if (IsPlayerDetected.Value)
        {
            return Status.Success;
        }
        _distance = Vector2.Distance(Target.Value.transform.position, Self.Value.transform.position);

        if (_direction != Direction.Value)
        {
            ChangeDirection();
        }

        return Status.Running;
    }

    private void ChangeDirection()
    {
        _direction = Direction.Value;
        _rigidbody2D.linearVelocityX = _direction * MoveSpeed.Value;
    }
}

