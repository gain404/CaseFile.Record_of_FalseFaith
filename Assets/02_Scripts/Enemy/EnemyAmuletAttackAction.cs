using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "EnemyAmuletAttack", story: "attack with [AmuletProjectile] at [AmuletFirePoint] based on [IsPhase2]", category: "Action", id: "47215c0a9bb3b7778d4fbd6571652ef2")]
public partial class EnemyAmuletAttackAction : Action
{
    [SerializeReference] public BlackboardVariable<GameObject> AmuletProjectile;
    [SerializeReference] public BlackboardVariable<Transform> AmuletFirePoint;
    [SerializeReference] public BlackboardVariable<bool> IsPhase2;

    private float _timeBetweenShots = 0.5f;
    private int _shotsFired = 0;
    private int _shotsToFire = 2;
    private float _nextFireTime = 0f;
    private GameObject _bossGameObject;
    private PoolManager _poolManager;

    protected override Status OnStart()
    {
        if (_poolManager == null)
        {
            _poolManager = PoolManager.Instance;
        }
        
        _shotsFired = 0;
        _nextFireTime = Time.time + 0.05f;

        _shotsToFire = IsPhase2.Value ? 3 : 2;

        return Status.Running;
    }

    protected override Status OnUpdate()
    {
        if (_shotsFired < _shotsToFire && Time.time >= _nextFireTime)
        {
            FireProjectile();
            _shotsFired++;
            _nextFireTime = Time.time + _timeBetweenShots;
        }

        if (_shotsFired >= _shotsToFire)
        {
            return Status.Success;
        }

        return Status.Running;
    }

    private void FireProjectile()
    {
        _poolManager.Get(PoolKey.WindProjectile, AmuletFirePoint.Value.position, AmuletFirePoint.Value.rotation);
    }
}

