using System;
using UnityEngine;
using DG.Tweening;

public class HairProjectile : MonoBehaviour
{
    [SerializeField] private int damage = 1;
    [SerializeField] private LayerMask hitLayerMask;
    [SerializeField] private HairZone _hairZone;

    private Animator _animator;
    private readonly int _hairAttack = Animator.StringToHash("HairAttack");

    private void Awake()
    {
        _animator = gameObject.GetComponent<Animator>();
    }

    public void HairAttack()
    {
        _animator.SetTrigger(_hairAttack);
    }

    private void FinishAttack()
    {
        _hairZone.CloseZone();
    }
    
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (((1 << collision.gameObject.layer) & hitLayerMask) != 0)
        {
            //플레이어 데미지
        }
    }
}
