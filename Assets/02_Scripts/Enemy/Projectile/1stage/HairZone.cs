using UnityEngine;
using DG.Tweening;

public class HairZone: MonoBehaviour
{
    [Header("Projectile Settings")] 
    [SerializeField] private HairProjectile hairProjectile;
    [SerializeField] private GameObject hairProjectileController;
    
    private PoolManager _poolManager;
    private Animator _zoneAnimator;
    private readonly int _isFinish = Animator.StringToHash("IsFinish");
    

    private void Awake()
    {
        _poolManager = PoolManager.Instance;
        _zoneAnimator = gameObject.GetComponent<Animator>();
    }

    private void OnEnable()
    {
        DOVirtual.DelayedCall(0.5f, hairProjectile.HairAttack);
    }

    public void CloseZone()
    {
        _zoneAnimator.SetTrigger(_isFinish);
    }
    
    private void ReturnProjectile()
    {
        _poolManager.Return(PoolKey.HairProjectile, hairProjectileController);
    }
}
