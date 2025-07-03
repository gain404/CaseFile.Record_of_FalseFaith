using System;
using System.Collections.Generic;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "GhostAttack", story: "attack with [GhostProjectile] at [GhostFirePoint] based on [IsPhase2]", category: "Action", id: "585cb2a4456a1783b4343074fa8d2b73")]
public partial class GhostAttackAction : Action
{
    [SerializeReference] public BlackboardVariable<GameObject> GhostProjectile;
    [SerializeReference] public BlackboardVariable<List<GameObject>> GhostFirePoint;
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

        _shotsToFire = IsPhase2.Value ? 5 : 3;

        for (int i = 0; i < _shotsToFire; i++)
        {
            FireProjectile(GhostFirePoint.Value[i]);
        }
        return Status.Success;
    }

    private void FireProjectile(GameObject point)
    {
        _poolManager.Get(PoolKey.GhostProjectile, point.transform.position, point.transform.rotation);
    }
}

