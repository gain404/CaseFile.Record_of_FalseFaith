using System;
using UnityEngine;

public class BulletProjectile : MonoBehaviour
{
    public int damage;
    public float damageDistance;
    
    [SerializeField] private LayerMask layerMask;
    private EnemyHealth _enemyHealth;
    private Rigidbody2D _rb;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
    }
    
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (((1 << collision.gameObject.layer) & layerMask) != 0)
        {
            if (collision.gameObject.TryGetComponent<IDamagable>(out var damagable))
            {
                damagable.TakeDamage(damage);
            }
        }
        ReturnProjectile();
    }

    private void ReturnProjectile()
    {
        PoolManager.Instance.Return(PoolKey.PlayerAmuletProjectile, gameObject);
    }

    public void ThrowTalismanProjectile(Vector2 projectileTransform, float speed)
    {
        _rb.linearVelocity = projectileTransform * speed;
    }
}
