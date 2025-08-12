using DG.Tweening;
using UnityEngine;

public class HairProjectile : MonoBehaviour
{
    [SerializeField] private int damage = 1;
    [SerializeField] private LayerMask hitLayerMask;
    [SerializeField] private AudioClip attackClip;
    [SerializeField] private HairProjectileContainer hairProjectileContainer;
    
    private Animator _animator;
    private SoundManager _soundManager;
    private readonly int _hairAttack = Animator.StringToHash("HairAttack");
    private bool _isDamage;

    private void Awake()
    {
        _animator = gameObject.GetComponent<Animator>();
        _soundManager = SoundManager.Instance;
        _isDamage = false;
    }

    private void OnEnable()
    {
        _isDamage = false;
        DOVirtual.DelayedCall(0.5f, () => _animator.SetTrigger(_hairAttack));
        DOVirtual.DelayedCall(3.0f, () => gameObject.SetActive(false));
    }
    
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(_isDamage) return;
        if (((1 << collision.gameObject.layer) & hitLayerMask) == 0) return;
        if (collision.TryGetComponent(out PlayerStat playerStat))
        {
            playerStat.TakeDamage(1);
            _isDamage = true;
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
