using DG.Tweening;
using UnityEngine;

public class TutorialEnemy : MonoBehaviour
{
    [SerializeField] private PolygonCollider2D meleeAttackCollider2D;
    [SerializeField] private MeleeAttack meleeAttack;
    [SerializeField] private EnemyController enemyController;
    [SerializeField] private Animator animator;
    [SerializeField] private SpriteRenderer spriteRenderer;
    [Header("Sound")]
    [SerializeField] private AudioClip attackClip;
    [SerializeField] private AudioClip dieClip;
    [SerializeField] private AudioClip moveClip;
    
    private static readonly int IsDie = Animator.StringToHash("IsDie");
    private SoundManager _soundManager;

    private void Start()
    {
        meleeAttackCollider2D.enabled = false;
        enemyController.DieAction += Die;
        _soundManager = SoundManager.Instance;
    }
    private void MeleeAttack()
    {
        meleeAttackCollider2D.enabled = true;
    }
    private void MeleeAttackFinish()
    {
        meleeAttackCollider2D.enabled = false;
    }
    private void Die()
    {
        animator.SetBool(IsDie,true);
        spriteRenderer.DOFade(0.9f, 1.0f).OnComplete(() => gameObject.SetActive(false));
    }

    private void AttackSfx()
    {
        _soundManager.PlaySFX(attackClip);
    }
    private void DieSfx()
    {
        _soundManager.PlaySFX(dieClip);
    }
    private void MoveSfx()
    {
        _soundManager.PlaySFX(moveClip);
    }
}
