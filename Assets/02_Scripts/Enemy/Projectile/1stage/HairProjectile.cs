using DG.Tweening;
using UnityEngine;

public class HairProjectile : MonoBehaviour
{
    [SerializeField] private int damage = 1;
    [SerializeField] private LayerMask hitLayerMask;
    [SerializeField] private AudioClip attackClip;
    [SerializeField] private HairProjectileContainer hairProjectileContainer;
    
    private Animator _animator;
    private PoolManager _poolManager;
    private SoundManager _soundManager;
    private readonly int _hairAttack = Animator.StringToHash("HairAttack");

    private void Awake()
    {
        _animator = gameObject.GetComponent<Animator>();
        _poolManager = PoolManager.Instance;
        _soundManager = SoundManager.Instance;
    }

    private void OnEnable()
    {
        _animator = gameObject.GetComponent<Animator>();
        _poolManager = PoolManager.Instance;
        _soundManager = SoundManager.Instance;
        DOVirtual.DelayedCall(0.5f, () => _animator.SetTrigger(_hairAttack));
    }
    
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (((1 << collision.gameObject.layer) & hitLayerMask) != 0)
        {
            if (collision.TryGetComponent(out PlayerStat playerStat))
            {
                playerStat.TakeDamage(1);
            }
        }
    }
    private void Finish()
    {
        gameObject.SetActive(false);
        hairProjectileContainer.FinishAttack();
    }
    
    public void AttackSfxPlay()
    {
        _soundManager.PlaySFX(attackClip);
    }
}
