using Unity.Behavior;
using UnityEngine;

public class AttackDistanceCollider : MonoBehaviour
{
    [SerializeField] private BehaviorGraphAgent agent;
    [SerializeField] private LayerMask playerLayerMask;
    [SerializeField] private string boolString;
    
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (((1 << collision.gameObject.layer) & playerLayerMask) != 0)
        {
            agent.BlackboardReference.SetVariableValue(boolString, true);
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
