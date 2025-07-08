using System;
using UnityEngine;

public class MeleeAttack : MonoBehaviour
{
    [SerializeField] private LayerMask mask;
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (((1 << other.gameObject.layer) & mask) != 0)
        {
            if (other.TryGetComponent(out StatManager statManager))
            {
                statManager.TakeDamage(1);
            }
        }
    }
}
