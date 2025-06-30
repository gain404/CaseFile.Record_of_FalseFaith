using DG.Tweening;
using UnityEngine;

public class GhostProjectile : MonoBehaviour
{
    [Header("Projectile Settings")]
    [SerializeField] private int damage = 1;
    [SerializeField] private float returnTime = 5f;
    [SerializeField] private LayerMask hitLayerMask;
    
    private PoolManager _poolManager;
    private GameObject _player;
    private Tween _moveTween;

    private void Awake()
    {
        _poolManager = PoolManager.Instance;
        _player = GameObject.FindWithTag("Player");
    }

    private void OnEnable()
    {
        DOVirtual.DelayedCall(returnTime, ReturnProjectile).SetTarget(this);
        RepeatFollow();
    }

    void RepeatFollow()
    {
        DOVirtual.DelayedCall(0.5f, () =>
        {
            if (_player == null) return;

            Vector3 targetPos = _player.transform.position;
            _moveTween?.Kill();
            _moveTween = transform.DOMove(targetPos, 0.5f).SetEase(Ease.Linear);

            RepeatFollow();
        }).SetTarget(this);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (((1 << collision.gameObject.layer) & hitLayerMask) != 0)
        {
            // 데미지 처리
            ReturnProjectile();
        }
    }

    private void OnDisable()
    {
        _moveTween?.Kill();
        DOTween.Kill(this);
    }

    private void ReturnProjectile()
    {
        _poolManager.Return(PoolKey.WindProjectile, gameObject);
    }
}