using System;
using UnityEngine;

public class MeleeAttack : MonoBehaviour
{
    public bool isDamaged;
    
    [SerializeField] private LayerMask mask;
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (((1 << other.gameObject.layer) & mask) != 0)
        {
            if (other.TryGetComponent(out PlayerStat playerStat))
            {
                playerStat.TakeDamage(1);
                isDamaged = true;
            }
        }
    }
}
