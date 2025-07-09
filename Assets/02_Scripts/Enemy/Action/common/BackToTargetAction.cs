using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "Back to Target", story: "[Self] go back to [Target] [MeleeAttackDistance]", category: "Action", id: "9b0ca106af92ce6c291445ba33631c9f")]
public partial class BackToTargetAction : Action
{
    [SerializeReference] public BlackboardVariable<GameObject> Self;
    [SerializeReference] public BlackboardVariable<GameObject> Target;
    [SerializeReference] public BlackboardVariable<float> MeleeAttackDistance;

    private LayerMask _layerMask;
    
    protected override Status OnStart()
    {
        _layerMask = LayerMask.GetMask("Wall");
        Transform targetTransform = Target.Value.transform;
        Transform selfTransform = Self.Value.transform;
        int targetFace = targetTransform.localScale.x > 0 ? 1 : -1;
        
        RaycastHit2D hit = Physics2D.Raycast(Target.Value.transform.position, new Vector2(-targetFace,0),9.0f,_layerMask);

        if (hit.collider != null)
        {
            targetFace *= -1;
        }
        
        selfTransform.position = new Vector3(targetTransform.position.x - MeleeAttackDistance.Value * targetFace,
            selfTransform.position.y + 7.7f, selfTransform.position.z);
        
        return Status.Success;
    }
}

