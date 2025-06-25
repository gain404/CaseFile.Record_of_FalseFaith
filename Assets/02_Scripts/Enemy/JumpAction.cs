using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;
using Random = System.Random;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "Jump", story: "[Self] Jump [Room] [jumpCount]", category: "Action", id: "43fbb0a0c7718811425f745e0952091a")]
public partial class JumpAction : Action
{
    [SerializeReference] public BlackboardVariable<GameObject> Self;
    [SerializeReference] public BlackboardVariable<GameObject> Room;
    [SerializeReference] public BlackboardVariable<int> JumpCount;

    private float _jumpForce = 5.0f;
    private float _horizontalForce;//수치 조정 필요
    private float _jumpTime = 3.0f;
    private Rigidbody2D _rigidbody2D;

    protected override Status OnStart()
    {
        if (Self.Value == null)
        {
            return Status.Failure;
        }
        if (_rigidbody2D == null)
        {
            _rigidbody2D = Self.Value.GetComponent<Rigidbody2D>();
        }
        StartJump();
        return Status.Running;
    }

    private void StartJump()
    {
        var direction = 1;//여기 개선
        _rigidbody2D.AddForce(new Vector2(_horizontalForce*direction, _jumpForce),ForceMode2D.Impulse);
    }

    protected override Status OnUpdate()
    {
        return Status.Success;
    }

    protected override void OnEnd()
    {
        JumpCount.Value++;
    }
}

