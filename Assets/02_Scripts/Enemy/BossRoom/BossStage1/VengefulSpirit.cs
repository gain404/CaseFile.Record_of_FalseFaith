using System;
using UnityEngine;

public class VengefulSpirit : MonoBehaviour
{
    [SerializeField] private PolygonCollider2D meleeAttackCollider2D;
    [SerializeField] private PolygonCollider2D biteAttackCollider2D;
    [SerializeField] private MeleeAttack meleeAttack;
    [SerializeField] private MeleeAttack biteAttack;
    [SerializeField] private SfxPlayer sfxPlayer;

    private void Start()
    {
        meleeAttackCollider2D.enabled = false;
        biteAttackCollider2D.enabled = false;
    }

    private void MeleeAttack()
    {
        meleeAttackCollider2D.enabled = true;
    }

    private void MeleeAttackFinish()
    {
        meleeAttackCollider2D.enabled = false;
    }

    private void SoundPlay()
    {
        if (meleeAttack.isDamaged)
        {
            sfxPlayer.PlaySfx(SfxName.MeleeAttackSuccess);
            meleeAttack.isDamaged = false;
        }
        else
        {
            sfxPlayer.PlaySfx(SfxName.MeleeAttackFailed);
        }
        if (biteAttack.isDamaged)
        {
            sfxPlayer.PlaySfx(SfxName.BiteSuccess);
            biteAttack.isDamaged = false;
        }
        else
        {
            sfxPlayer.PlaySfx(SfxName.BiteFailed);
        }
    }

    private void BiteAttack()
    {
        biteAttackCollider2D.enabled = true;
    }
    private void BiteAttackFinish()
    {
        biteAttackCollider2D.enabled = false;
    }
}
