using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;
using DG.Tweening;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "MeleeAttack", story: "[Self] set [attackTrigger] on and Wait for [duration] seconds", category: "Action", id: "630e8b7ae158ae0f4ede33fb387b88ac")]
public partial class MeleeAttackAction : Action
{
    [SerializeReference] public BlackboardVariable<GameObject> Self;
    [SerializeReference] public BlackboardVariable<string> AttackTrigger;
    [SerializeReference] public BlackboardVariable<float> Duration;
    private bool _isAttack;
    private Animator _animator;

    protected override Status OnStart()
    {
        if (_animator == null)
        {
            _animator = Self.Value.GetComponent<Animator>();
        }

        _isAttack = false;
        _animator.SetTrigger(AttackTrigger);
        DOVirtual.DelayedCall(Duration, () =>
        {
            _isAttack = true;
        });
        
        return Status.Running;
    }

    protected override Status OnUpdate()
    {
        return _isAttack ? Status.Success : Status.Running;
    }
}

