using DG.Tweening;
using UnityEngine;
using UnityEngine.Rendering.Universal;

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
    [SerializeField] private Light2D globalLight;
    [Header("Sound")]
    [SerializeField] private AudioClip meleeAttackClip;
    [SerializeField] private AudioClip biteAttackClip;
    [SerializeField] private AudioClip hairAttackClip;
    [SerializeField] private AudioClip howlingAttackClip;
    [SerializeField] private AudioClip dieClip;
    [SerializeField] private AudioClip moveStart;
    [SerializeField] private AudioClip shadowDown;
    

    private PoolManager _poolManager;
    private SoundManager _soundManager;
    private static readonly int IsDie = Animator.StringToHash("IsDie");
    private void Start()
    {
        _poolManager = PoolManager.Instance;
        _soundManager = SoundManager.Instance;
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
        globalLight.color = Color.white;
    }
    
    private void MeleeAttackSfx()
    {
        _soundManager.PlaySFX(meleeAttackClip);
    }
    private void BiteAttackSfx()
    {
        _soundManager.PlaySFX(biteAttackClip);
    }
    private void HairAttackSfx()
    {
        _soundManager.PlaySFX(hairAttackClip);
    }
    private void HowlingAttackSfx()
    {
        _soundManager.PlaySFX(howlingAttackClip);
    }
    private void DieSfx()
    {
        _soundManager.PlaySFX(dieClip);
    }
    private void MoveStartSfx()
    {
        _soundManager.PlaySFX(moveStart);
    }

    private void ShadowDown()
    {
        _soundManager.PlaySFX(shadowDown);
    }
}
