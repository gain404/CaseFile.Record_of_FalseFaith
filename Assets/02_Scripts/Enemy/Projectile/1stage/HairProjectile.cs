using UnityEngine;
using DG.Tweening;

public class HairProjectile : MonoBehaviour
{
    

    [Header("Projectile Settings")]
    [SerializeField] private int damage = 1;
    [SerializeField] private LayerMask hitLayerMask;
    
    private PoolManager _poolManager;
    private Tween _moveTween;
    private Animator _animator;
    private readonly int _hairAttack = Animator.StringToHash("HairAttack");

    private void Awake()
    {
        _poolManager = PoolManager.Instance;
        _animator = gameObject.GetComponent<Animator>();
    }

    private void OnEnable()
    {
        DOVirtual.DelayedCall(0.5f, () => _animator.SetTrigger(_hairAttack));
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (((1 << collision.gameObject.layer) & hitLayerMask) != 0)
        {
            //플레이어 데미지
        }
    }
    
    private void ReturnProjectile()
    {
        _poolManager.Return(PoolKey.HairProjectile, gameObject);
    }
}
