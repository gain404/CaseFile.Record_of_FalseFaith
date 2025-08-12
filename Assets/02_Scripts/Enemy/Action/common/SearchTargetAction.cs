using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "SearchTarget", story: "[Self] search [Target] with [distance] for [IsTargetDetected]", category: "Action", id: "fd399354ba76caced6fa4c6fa5180d61")]
public partial class SearchTargetAction : Action
{
    [SerializeReference] public BlackboardVariable<GameObject> Self;
    [SerializeReference] public BlackboardVariable<GameObject> Target;
    [SerializeReference] public BlackboardVariable<float> Distance;
    [SerializeReference] public BlackboardVariable<bool> IsTargetDetected;
    
    protected override Status OnStart()
    { 
        Vector3 startPosition = Self.Value.transform.parent.position;
        
        float selfPosX = Self.Value.transform.position.x;
        if (Target.Value == null)
        {
            Target.Value = GameObject.FindGameObjectWithTag("Player");
        }
        float playerPosX = Target.Value.transform.position.x;
        if (Mathf.Abs(startPosition.y - Target.Value.transform.position.y) > 70.0f)
        {
            return Status.Success;
        }
        
        float distance = Mathf.Abs(startPosition.x - playerPosX);
        
        if (distance<= Distance.Value)
        {
            IsTargetDetected.Value = true;
        }
        else if(distance > Distance.Value + 10)
        {
            IsTargetDetected.Value = false;
        }
        else if (selfPosX - playerPosX < 8)
        {
            IsTargetDetected.Value = true;
        }
        else
        {
            IsTargetDetected.Value = false;
        }
        
        return Status.Success;
    }
}

