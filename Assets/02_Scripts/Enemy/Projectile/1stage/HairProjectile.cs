using DG.Tweening;
using UnityEngine;

public class HairProjectile : MonoBehaviour
{
    [SerializeField] private int damage = 1;
    [SerializeField] private LayerMask hitLayerMask;
    [SerializeField] private GameObject hairProjectileController;
    [SerializeField] private AudioClip attackClip;
    
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
        DOVirtual.DelayedCall(0.5f, () => _animator.SetTrigger(_hairAttack));
    }

    private void FinishAttack()
    {
        _poolManager.Return(PoolKey.HairProjectile, hairProjectileController);
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

    public void AttackSfxPlay()
    {
        _soundManager.PlaySFX(attackClip);
    }
}
