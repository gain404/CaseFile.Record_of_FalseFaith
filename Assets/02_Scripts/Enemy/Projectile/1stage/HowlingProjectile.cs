using System;
using UnityEngine;

public class HowlingProjectile : MonoBehaviour
{
    [SerializeField] private int damage = 1;
    [SerializeField] private LayerMask hitLayerMask;
    [SerializeField] private LayerMask lightLayerMask;

    private Animator _animator;
    private readonly int _hairAttack = Animator.StringToHash("HairAttack");

    private void Awake()
    {
        _animator = gameObject.GetComponent<Animator>();
    }

    private void OnEnable()
    {
        
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (((1 << collision.gameObject.layer) & hitLayerMask) != 0)
        {
            //데미지
        }
        else if (((1 << collision.gameObject.layer) & lightLayerMask) != 0)
        {
            
        }
    }
}
