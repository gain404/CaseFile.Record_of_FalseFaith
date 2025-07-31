using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "BackToStartPosition", story: "[Self] back to StartPostion and heal [Currenthealth] to [Maxhealth] with [IsTargetDetected]", category: "Action", id: "8deb8b193a75fb2fc0ba481b82f25b34")]
public partial class BackToStartPositionAction : Action
{
    [SerializeReference] public BlackboardVariable<GameObject> Self;
    [SerializeReference] public BlackboardVariable<float> Currenthealth;
    [SerializeReference] public BlackboardVariable<float> Maxhealth;
    [SerializeReference] public BlackboardVariable<bool> IsTargetDetected;
    private float _startPosition;
    private float _posX;
    private Rigidbody2D _rigidbody2D;
    
    protected override Status OnStart()
    {
        _posX = Self.Value.transform.position.x;
        _startPosition = Self.Value.transform.parent.position.x;
        if (_rigidbody2D == null)
        {
            _rigidbody2D = Self.Value.GetComponent<Rigidbody2D>();
        }
        
        if (IsStartPosition())
        {
            return Status.Success;
        }

        if (_posX > _startPosition)
        {
            _rigidbody2D.linearVelocityX = -1.0f;
        }
        else
        {
            _rigidbody2D.linearVelocityX = 1.0f;
        }
        return Status.Running;
    }

    protected override Status OnUpdate()
    {
        if (IsTargetDetected.Value || IsStartPosition())
        {
            return Status.Success;
        }

        return Status.Running;
    }

    private bool IsStartPosition()
    {
        _posX = Self.Value.transform.position.x;
        if (!(Mathf.Abs(_posX - _startPosition) < 1.0f))
        {
            return false;
        }
        Currenthealth.Value = Maxhealth.Value;
        return true;
    }
}

