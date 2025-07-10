using System;
using Unity.Cinemachine;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerCamera : MonoBehaviour
{
    [SerializeField] private LayerMask hitLayerMask;
    private CinemachineConfiner2D _cinemachineConfiner2D;
    private void Awake()
    {
        _cinemachineConfiner2D = GetComponent<CinemachineConfiner2D>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (((1 << other.gameObject.layer) & hitLayerMask) != 0)
        {
            if (other.TryGetComponent(out BoxCollider2D boxCollider2D))
            {
                _cinemachineConfiner2D.BoundingShape2D = boxCollider2D;
            }
        }
    }
}
