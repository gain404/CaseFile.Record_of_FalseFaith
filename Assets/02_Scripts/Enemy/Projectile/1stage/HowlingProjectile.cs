using DG.Tweening;
using UnityEngine;

public class HowlingProjectile : MonoBehaviour
{
    [SerializeField] private int damage = 1;
    [SerializeField] private LayerMask hitLayerMask;
    [SerializeField] private LayerMask lightLayerMask;

    private Animator _animator;
    private bool _isStartHowling;
    private PoolManager _poolManager;

    private void Awake()
    {
        _animator = gameObject.GetComponent<Animator>();
        _poolManager = PoolManager.Instance;
    }

    private void OnEnable()
    {
        transform.localScale = new Vector3(1f, 1f, 0);
        _isStartHowling = true;
        transform.DOScale(new Vector3(40f, 40f, 0f), 5f)
            .OnComplete(FinishHowling);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (((1 << collision.gameObject.layer) & hitLayerMask) != 0)
        {
            if (collision.TryGetComponent(out StatManager statManager))
            {
                statManager.TakeDamage(1);
            }
        }

        if (_isStartHowling)
        {
            if (collision.gameObject.TryGetComponent(out BossRoomLight bossRoomLight))
            {
                bossRoomLight.BreakLight();
                if (bossRoomLight.BreakCount == 0)
                {
                    return;
                }
                _isStartHowling = false;
            }
        }
    }

    private void FinishHowling()
    {
        _poolManager.Return(PoolKey.HowlingProjectile, gameObject);
    }
}
