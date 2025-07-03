using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "BottomAttack", story: "Attack with [Projectile] at [AttackZone] with [Distance] and [Count]", category: "Action", id: "f0e40a3065fe9494a50dc698ae77b6eb")]
public partial class BottomAttackAction : Action
{
    [SerializeReference] public BlackboardVariable<int> Projectile;
    [SerializeReference] public BlackboardVariable<Transform> AttackZone;
    [SerializeReference] public BlackboardVariable<float> Distance;
    [SerializeReference] public BlackboardVariable<int> Count;
    private PoolManager _poolManager;
    protected override Status OnStart()
    {
        if (_poolManager == null)
        {
            _poolManager = PoolManager.Instance;
        }
        
        for (int i = 0; i < Count; i++)
        {
            Vector3 pos = new Vector3(AttackZone.Value.position.x + (Distance.Value * i),
                AttackZone.Value.position.y,
                AttackZone.Value.position.z);
            _poolManager.Get((PoolKey)Projectile.Value,pos, Quaternion.Euler(0,0,0));
        }

        return Status.Success;
    }
}

