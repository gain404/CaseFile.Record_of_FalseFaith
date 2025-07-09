using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "HowlingAttack", story: "[HowlingProjectile] spawn at [HowlingZone]", category: "Action", id: "f91a249aff7079a4fe5eb44840a21c7e")]
public partial class HowlingAttackAction : Action
{
    [SerializeReference] public BlackboardVariable<int> HowlingProjectile;
    [SerializeReference] public BlackboardVariable<Transform> HowlingZone;
    private PoolManager _poolManager;

    protected override Status OnStart()
    {
        if (_poolManager == null)
        {
            _poolManager = PoolManager.Instance;
        }

        _poolManager.Get((PoolKey)HowlingProjectile.Value, HowlingZone.Value.position, Quaternion.Euler(0, 0, 0));
        return Status.Success;
    }
}

