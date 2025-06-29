using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "WindAttack", story: "[Self] attack with [WindProjectile] at [WindFirePoint] based on [IsPhase2]", category: "Action", id: "1ed707ff4893137d794a3f6a4e9e7ea2")]
public partial class WindAttackAction : Action
{
    [SerializeReference] public BlackboardVariable<GameObject> Self;
    [SerializeReference] public BlackboardVariable<GameObject> WindProjectile;
    [SerializeReference] public BlackboardVariable<Transform> WindFirePoint;
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
        
        if (_bossGameObject == null)
        {
            _bossGameObject = Self.Value;
        }
        
        _shotsFired = 0;
        _nextFireTime = Time.time + 0.1f;

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
        _poolManager.Get(PoolKey.WindProjectile, WindFirePoint.Value.localPosition, WindFirePoint.Value.localRotation);
    }
}

