using System;
using UnityEngine;

public class MeleeAttack : MonoBehaviour
{
    [SerializeField] private AudioClip attackHitClip;
    [SerializeField] private LayerMask mask;
    
    private bool _isDamaged;

    private void OnEnable()
    {
        _isDamaged = false;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (_isDamaged) return;
        if (((1 << other.gameObject.layer) & mask) == 0) return;
        if (!other.TryGetComponent(out PlayerStat playerStat)) return;
        AttackHitSfx();
        playerStat.TakeDamage(1);
        _isDamaged = true;
    }
    
    private void AttackHitSfx()
    {
        SoundManager.Instance.PlaySFX(attackHitClip);
    }
}
