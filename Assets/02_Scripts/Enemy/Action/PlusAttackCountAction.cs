using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "PlusAttackCount", story: "plus [AttackCount] until [maxAttackCount] for [AttackIndex]", category: "Action", id: "a88474899e4689d398f12fcf2f5932f3")]
public partial class PlusAttackCountAction : Action
{
    [SerializeReference] public BlackboardVariable<int> AttackCount;
    [SerializeReference] public BlackboardVariable<int> MaxAttackCount;
    [SerializeReference] public BlackboardVariable<int> AttackIndex;
    protected override Status OnStart()
    {
        if (AttackCount.Value == MaxAttackCount.Value)
        {
            AttackCount.Value = 0;
            AttackIndex.Value = 10;
            return Status.Success;
        }
        AttackCount.Value++;
        return Status.Success;
    }
}

