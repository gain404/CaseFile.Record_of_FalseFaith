using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;
using DG.Tweening;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "MeleeAttackMove", story: "[Self] move to [Target] [MeleeAttackMoveSpeed] between [MeleeAttackDistance]", category: "Action", id: "e3b916fcc03aa7dc2152551d31d04051")]
public partial class MeleeAttackMoveAction : Action
{
    [SerializeReference] public BlackboardVariable<GameObject> Self;
    [SerializeReference] public BlackboardVariable<GameObject> Target;
    [SerializeReference] public BlackboardVariable<float> MeleeAttackMoveSpeed;
    [SerializeReference] public BlackboardVariable<float> MeleeAttackDistance;

    private bool _isMoveComplete;
    protected override Status OnStart()
    {
        _isMoveComplete = false;
        float endPos = Target.Value.transform.position.x > Self.Value.transform.position.x
            ? Target.Value.transform.position.x - MeleeAttackDistance
            : Target.Value.transform.position.x + MeleeAttackDistance;
        float distance = Math.Abs(endPos - Self.Value.transform.position.x);
        Self.Value.transform.DOMoveX(endPos, distance / MeleeAttackMoveSpeed)
            .SetEase(Ease.OutQuad)
            .OnComplete(() =>
            {
                _isMoveComplete = true;
            });
        return Status.Running;
    }

    protected override Status OnUpdate()
    {
        return _isMoveComplete ? Status.Success : Status.Running;
    }
}

