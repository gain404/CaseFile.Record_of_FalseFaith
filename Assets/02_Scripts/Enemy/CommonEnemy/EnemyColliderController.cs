using UnityEngine;
using Unity.Behavior;

public class EnemyColliderController : MonoBehaviour
{
    [SerializeField] private BehaviorGraphAgent agent;
    [SerializeField] private LayerMask hitLayerMask;
    [SerializeField] private string boolString;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (((1 << collision.gameObject.layer) & hitLayerMask) != 0)
        {
            agent.BlackboardReference.SetVariableValue(boolString, true);
        }
    }
    
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (((1 << collision.gameObject.layer) & hitLayerMask) != 0)
        {
            agent.BlackboardReference.SetVariableValue(boolString, false);
        }
    }
}
