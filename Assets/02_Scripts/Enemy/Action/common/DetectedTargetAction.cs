using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "DetectedTarget", story: "[Self] detected [Target] with [IsTargetDetected] with [MoveSpeed] and [StopAnim] for [AttackDistance]", category: "Action", id: "e5611cd9fbf8d1ec40ae4c0f367a7fa9")]
public partial class DetectedTargetAction : Action
{
    [SerializeReference] public BlackboardVariable<GameObject> Self;
    [SerializeReference] public BlackboardVariable<GameObject> Target;
    [SerializeReference] public BlackboardVariable<bool> IsTargetDetected;
    [SerializeReference] public BlackboardVariable<float> MoveSpeed;
    [SerializeReference] public BlackboardVariable<string> StopAnim;
    [SerializeReference] public BlackboardVariable<float> AttackDistance;
    private Rigidbody2D _rigidbody2D;
    private Animator _animator;
    private float _endPos;
    
    protected override Status OnStart()
    {
        if (_rigidbody2D == null)
        {
            _rigidbody2D = Self.Value.GetComponent<Rigidbody2D>();
        }

        if (_animator == null)
        {
            _animator = Self.Value.GetComponent<Animator>();
        }
        
        _endPos = Target.Value.transform.position.x > Self.Value.transform.position.x
            ? Target.Value.transform.position.x - AttackDistance.Value
            : Target.Value.transform.position.x + AttackDistance.Value;
        float direction = Mathf.Sign(_endPos - Self.Value.transform.position.x);
        _rigidbody2D.linearVelocityX = direction * MoveSpeed;
        return Status.Running;
    }

    protected override Status OnUpdate()
    {
        if (IsTargetDetected.Value)
        {
            _animator.SetBool(StopAnim.Value, false);
            return Status.Failure;
        }
        
        float currentX = Self.Value.transform.position.x;
        if (Mathf.Abs(currentX - _endPos) <= 0.5f) // 오차 범위
        {
            _animator.SetBool(StopAnim.Value, false);
            _rigidbody2D.linearVelocity = Vector2.zero;
            return Status.Success;
        }

        return Status.Running;
    }
}

