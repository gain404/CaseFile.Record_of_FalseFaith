using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;


[Serializable, GeneratePropertyBag]
[NodeDescription(name: "MeleeAttackMove", story: "[Self] move to [Target] [MeleeAttackMoveSpeed] between [MeleeAttackDistance]", category: "Action", id: "e3b916fcc03aa7dc2152551d31d04051")]
public partial class MeleeAttackMoveAction : Action
{
    [SerializeReference] public BlackboardVariable<GameObject> Self;
    [SerializeReference] public BlackboardVariable<GameObject> Target;
    [SerializeReference] public BlackboardVariable<float> MeleeAttackMoveSpeed;
    [SerializeReference] public BlackboardVariable<float> MeleeAttackDistance;

    private Rigidbody2D _rigidbody2D;
    private float _endPos;
    protected override Status OnStart()
    {
        if (_rigidbody2D == null)
        {
            _rigidbody2D = Self.Value.GetComponent<Rigidbody2D>();
        }
        
        _endPos = Target.Value.transform.position.x > Self.Value.transform.position.x
            ? Target.Value.transform.position.x - MeleeAttackDistance.Value
            : Target.Value.transform.position.x + MeleeAttackDistance.Value;
        float direction = Mathf.Sign(_endPos - Self.Value.transform.position.x);
        _rigidbody2D.linearVelocityX = direction * MeleeAttackMoveSpeed;
        return Status.Running;
    }
    
    protected override Status OnUpdate()
    {
        float currentX = Self.Value.transform.position.x;
        if (Mathf.Abs(currentX - _endPos) <= 0.5f) // 오차 범위
        {
            _rigidbody2D.linearVelocity = Vector2.zero;
            return Status.Success;
        }
        return Status.Running;
    }
    
}

