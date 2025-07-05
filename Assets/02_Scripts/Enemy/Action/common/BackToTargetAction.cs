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

    private float _underGround;
    protected override Status OnStart()
    {
        Transform targetTransform = Target.Value.transform;
        int targetFace = targetTransform.localScale.x > 0 ? 1 : -1;
        Self.Value.transform.position = new Vector3(targetTransform.position.x - MeleeAttackDistance.Value * targetFace,
            targetTransform.position.y - _underGround, targetTransform.position.z);
        
        return Status.Success;
    }
}

