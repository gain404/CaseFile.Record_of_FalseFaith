using System;
using Unity.Behavior;
using UnityEngine;

public class AreaCollider : MonoBehaviour
{
    [SerializeField] private LayerMask playerLayerMask;
    [SerializeField] private GameObject enemy;

    private Rigidbody2D _enemyRigidbody2D;
    private BehaviorGraphAgent _agent;
    private void Start()
    {
        _enemyRigidbody2D = enemy.GetComponent<Rigidbody2D>();
        _agent = enemy.GetComponent<BehaviorGraphAgent>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (((1 << other.gameObject.layer) & playerLayerMask) != 0)
        {
            _agent.BlackboardReference.SetVariableValue("IsTargetDetected",true);
        }

        if (other.gameObject != enemy) return;
        float velocityX = _enemyRigidbody2D.linearVelocityX;
        _enemyRigidbody2D.linearVelocityX = -velocityX;
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (((1 << other.gameObject.layer) & playerLayerMask) != 0)
        {
            _agent.BlackboardReference.SetVariableValue("IsTargetDetected",false);
        }
    }
}
