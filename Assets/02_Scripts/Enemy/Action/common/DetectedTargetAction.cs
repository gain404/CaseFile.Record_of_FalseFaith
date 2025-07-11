using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "DetectedTarget", story: "[Self] detected [Target] with [IsTargetDetected] and [IsTargetInAttackDistance] move with [MoveSpeed]", category: "Action", id: "e5611cd9fbf8d1ec40ae4c0f367a7fa9")]
public partial class DetectedTargetAction : Action
{
    [SerializeReference] public BlackboardVariable<GameObject> Self;
    [SerializeReference] public BlackboardVariable<GameObject> Target;
    [SerializeReference] public BlackboardVariable<bool> IsTargetDetected;
    [SerializeReference] public BlackboardVariable<bool> IsTargetInAttackDistance;
    [SerializeReference] public BlackboardVariable<float> MoveSpeed;
    private Rigidbody2D _rigidbody2D;
    
    protected override Status OnStart()
    {
        if (_rigidbody2D == null)
        {
            _rigidbody2D = Self.Value.GetComponent<Rigidbody2D>();
        }
        
        float direction = Mathf.Sign(Target.Value.transform.position.x - Self.Value.transform.position.x);
        _rigidbody2D.linearVelocityX = direction * MoveSpeed;
        return Status.Running;
    }

    protected override Status OnUpdate()
    {
        if (!IsTargetDetected.Value)
        {
            return Status.Success;
        }

        if (IsTargetInAttackDistance)
        {
            _rigidbody2D.linearVelocity = Vector2.zero;
            return Status.Success;
        }

        return Status.Running;
    }
}

