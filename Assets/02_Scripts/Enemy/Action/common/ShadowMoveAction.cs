using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;
using DG.Tweening;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "ShadowMove", story: "[Self] ShadowMove [Up_or_Down]", category: "Action", id: "8bf8e13a1b029cc2bf0cf67b4fd61347")]
public partial class ShadowMoveAction : Action
{
    [SerializeReference] public BlackboardVariable<GameObject> Self;
    [SerializeReference] public BlackboardVariable<bool> Up_or_Down;

    protected override Status OnStart()
    {
        Transform selfTransform = Self.Value.transform;
        float direction = Up_or_Down.Value ? 7.7f : -7.7f;
        selfTransform.DOMoveY(selfTransform.position.y + direction, 0.8f);
        return Status.Success;
    }

}

