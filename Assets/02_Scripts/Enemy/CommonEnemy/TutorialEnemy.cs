using DG.Tweening;
using UnityEngine;

public class TutorialEnemy : MonoBehaviour
{
    [SerializeField] private PolygonCollider2D meleeAttackCollider2D;
    [SerializeField] private MeleeAttack meleeAttack;
    [SerializeField] private EnemyController enemyController;
    [SerializeField] private Animator animator;
    [SerializeField] private SpriteRenderer spriteRenderer;
    
    private static readonly int IsDie = Animator.StringToHash("IsDie");

    private void Start()
    {
        meleeAttackCollider2D.enabled = false;
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
    private void Die()
    {
        animator.SetBool(IsDie,true);
        spriteRenderer.DOFade(0.9f, 1.3f).OnComplete(() => gameObject.SetActive(false));
    }
}
