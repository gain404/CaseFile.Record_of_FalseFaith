using System;
using UnityEngine;

public class HowlingProjectile : MonoBehaviour
{
    [SerializeField] private int damage = 1;
    [SerializeField] private LayerMask hitLayerMask;
    [SerializeField] private LayerMask lightLayerMask;

    private Animator _animator;
    private bool _isStartHowling;
    private readonly int _hairAttack = Animator.StringToHash("HairAttack");

    private void Awake()
    {
        _animator = gameObject.GetComponent<Animator>();
    }

    private void OnEnable()
    {
        _isStartHowling = true;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (((1 << collision.gameObject.layer) & hitLayerMask) != 0)
        {
            //데미지
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
}
