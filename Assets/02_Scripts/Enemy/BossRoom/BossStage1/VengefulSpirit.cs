using DG.Tweening;
using UnityEngine;

public class VengefulSpirit : MonoBehaviour
{
    [SerializeField] private PolygonCollider2D meleeAttackCollider2D;
    [SerializeField] private PolygonCollider2D biteAttackCollider2D;
    [SerializeField] private MeleeAttack meleeAttack;
    [SerializeField] private MeleeAttack biteAttack;
    [SerializeField] private Transform howlingZone;
    [SerializeField] private EnemyController enemyController;
    [SerializeField] private Animator animator;
    [SerializeField] private SpriteRenderer spriteRenderer;
    //[SerializeField] private SfxPlayer sfxPlayer;

    private PoolManager _poolManager;
    private static readonly int IsDie = Animator.StringToHash("IsDie");
    private void Start()
    {
        _poolManager = PoolManager.Instance;
        meleeAttackCollider2D.enabled = false;
        biteAttackCollider2D.enabled = false;
        enemyController.DieAction += Die;
    }

    private void MeleeAttack()
    {
        meleeAttackCollider2D.enabled = true;
    }

    private void MeleeAttackFinish()
    {
        meleeAttackCollider2D.enabled = false;
    }

    private void BiteAttack()
    {
        biteAttackCollider2D.enabled = true;
    }
    private void BiteAttackFinish()
    {
        biteAttackCollider2D.enabled = false;
    }

    private void HowlingAttack()
    {
        _poolManager.Get(PoolKey.HowlingProjectile, howlingZone.position, Quaternion.Euler(0, 0, 0));
    }

    private void Die()
    {
        animator.SetBool(IsDie,true);
        spriteRenderer.DOFade(0.9f, 1.3f).OnComplete(() => gameObject.SetActive(false));
    }
    
    // private void SoundPlay()
    // {
    //     if (meleeAttack.isDamaged)
    //     {
    //         sfxPlayer.PlaySfx(SfxName.MeleeAttackSuccess);
    //         meleeAttack.isDamaged = false;
    //     }
    //     else
    //     {
    //         sfxPlayer.PlaySfx(SfxName.MeleeAttackFailed);
    //     }
    //     if (biteAttack.isDamaged)
    //     {
    //         sfxPlayer.PlaySfx(SfxName.BiteSuccess);
    //         biteAttack.isDamaged = false;
    //     }
    //     else
    //     {
    //         sfxPlayer.PlaySfx(SfxName.BiteFailed);
    //     }
    // }
}
