using System;
using UnityEngine;

public class WindProjectile : MonoBehaviour
{
    [Header("Projectile Settings")]
    [SerializeField] private int damage = 1;
    [SerializeField] private LayerMask hitLayerMask;
    
    private Rigidbody2D _rigidbody2D;
    private PoolManager _poolManager;

    private void Awake()
    {
        _rigidbody2D = GetComponent<Rigidbody2D>();
        _poolManager = PoolManager.Instance;
    }

    private void OnEnable()
    {
        if (transform.rotation.y > 90)
        {
            _rigidbody2D.linearVelocityX = 1.0f;//나중에 수정
        }
        else
        {
            _rigidbody2D.linearVelocityX = -1.0f;
        }
    }
    
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.layer == hitLayerMask)
        {
            //플레이어에 데미지
            _poolManager.Return(PoolKey.WindProjectile, this.gameObject);
        }
    }
}
