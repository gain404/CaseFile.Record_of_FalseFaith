using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "WindAttack", story: "[Self] attack with [WindProjectile] based on [IsPhase2]", category: "Action", id: "1ed707ff4893137d794a3f6a4e9e7ea2")]
public partial class WindAttackAction : Action
{
    [SerializeReference] public BlackboardVariable<GameObject> Self;
    [SerializeReference] public BlackboardVariable<GameObject> WindProjectile;
    [SerializeReference] public BlackboardVariable<bool> IsPhase2;

    
    public float timeBetweenShots = 0.5f;
    
    private int _shotsFired = 0;
    private int _shotsToFire = 2;
    private float _nextFireTime = 0f;
    private Transform _firePoint;
    private GameObject _bossGameObject;

    protected override Status OnStart()
    {
        if (_bossGameObject == null)
        {
            _bossGameObject = Self.Value;
        }

        if (_firePoint == null)
        {
            
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
            _nextFireTime = Time.time + timeBetweenShots;
        }

        if (_shotsFired >= _shotsToFire)
        {
            return Status.Success;
        }

        return Status.Running;
    }

    private void FireProjectile()
    {
        Transform bossTransform = _bossGameObject.transform;

        // GameObject projectile = GameObject.Instantiate(
        //     windProjectilePrefab,
        //     _firePoint.position,
        //     bossTransform.rotation
        //);

        // 필요 시 발사 방향 및 속도 설정
        //Rigidbody2D projectileRb = projectile.GetComponent<Rigidbody2D>();
        // if (projectileRb != null)
        // {
        //     float speed = 10f;
        //     Vector2 direction = bossTransform.right;
        //     projectileRb.velocity = direction * speed;
        }

}

