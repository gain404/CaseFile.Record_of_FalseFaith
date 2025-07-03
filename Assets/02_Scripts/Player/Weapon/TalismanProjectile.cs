using System;
using UnityEngine;

public class TalismanProjectile : MonoBehaviour
{
    public int damage;
    private EnemyHealth _enemyHealth;
    
    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Enemy"))
        {
            _enemyHealth.TakeDamage(damage);
        }
    }
}
