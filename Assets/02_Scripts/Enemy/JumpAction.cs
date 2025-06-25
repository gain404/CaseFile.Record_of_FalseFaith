using System;
using System.Collections;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "Jump", story: "[Self] jump to [Target] during [time]", category: "Action", id: "5e6d8bcf6dcedcecb5329b07581ee959")]
public partial class JumpAction : Action
{
    [SerializeReference] public BlackboardVariable<GameObject> Self;
    [SerializeReference] public BlackboardVariable<GameObject> Target;
    [SerializeReference] public BlackboardVariable<float> Time;
    [SerializeReference] private float jumpForce;
    [SerializeReference] private float jumpTime;
    [SerializeReference] private bool hasLanded;

    protected override Status OnStart()
    {
        //enemyAnimator.SetTrigger(animationTriggerName); 이것도 저기서 해줘야겠네
        //사잉에 wait넣어야하나?
        StartJump();
        return Status.Running;
    }

    private void StartJump()
    {
        
    }

    protected override Status OnUpdate()
    {
        return hasLanded ? Status.Success : Status.Running;
    }

    protected override void OnEnd()
    {
    }
}

