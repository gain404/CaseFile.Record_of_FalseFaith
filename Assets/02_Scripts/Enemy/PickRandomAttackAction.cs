using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;
using System.Collections.Generic;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "PickRandomAttackAction", story: "Choose [AttackIndex] with [LastAttackIndex]", category: "Action", id: "0f2ce86f9593302707cfff43e0d20fb4")]
public partial class PickRandomAttackAction : Action
{
    [SerializeReference] public BlackboardVariable<int> AttackIndex;
    [SerializeReference] public BlackboardVariable<int> LastAttackIndex;

    protected override Status OnStart()
    {
        int[] attackCandidates = new int[] { 0, 1, 2 };

        // 직전 공격 인덱스 가져오기
        int previousAttackIndex = LastAttackIndex.Value;

        // 직전과 동일한 인덱스를 제외
        List<int> availableCandidates = new List<int>();
        foreach (int index in attackCandidates)
        {
            if (index != previousAttackIndex)
            {
                availableCandidates.Add(index);
            }
        }

        // 랜덤으로 선택
        int selectedAttackIndex = availableCandidates[UnityEngine.Random.Range(0, availableCandidates.Count)];

        // Blackboard 업데이트
        AttackIndex.Value = selectedAttackIndex;
        LastAttackIndex.Value = selectedAttackIndex;

        return Status.Success;
    }
}

