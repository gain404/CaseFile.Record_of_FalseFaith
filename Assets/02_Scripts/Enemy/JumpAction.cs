using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;

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

    protected override Status OnStart()
    {
        return Status.Running;
    }

    private void StartJump()
    {
        
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

