using System;
using Unity.Behavior;
using UnityEngine;

public class EnemyAttackCollider : MonoBehaviour
{
    [SerializeField] private BehaviorGraphAgent agent;
    [SerializeField] private LayerMask playerLayerMask;
    [SerializeField] private LayerMask wallLayerMask;
    [SerializeField] private string boolString;

    private int _direction;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (((1 << collision.gameObject.layer) & playerLayerMask) != 0)
        {
            agent.BlackboardReference.SetVariableValue(boolString, true);
        }
        else if (((1 << collision.gameObject.layer) & wallLayerMask) != 0)
        {
            agent.BlackboardReference.GetVariableValue("Direction", out _direction);
            _direction *= -1;
            agent.BlackboardReference.SetVariableValue("Direction", _direction);
            var scale = transform.parent.localScale;
            scale.x = scale.x * -1;
            transform.parent.localScale = scale;
        }
    }
    
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (((1 << collision.gameObject.layer) & playerLayerMask) != 0)
        {
            agent.BlackboardReference.SetVariableValue(boolString, false);
        }
    }
}
