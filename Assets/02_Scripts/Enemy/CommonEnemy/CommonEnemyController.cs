using System.Collections.Generic;
using Unity.Behavior;
using UnityEngine;

public class CommonEnemyController : MonoBehaviour
{
    [SerializeField] private BehaviorGraphAgent agent;
    
    private GameObject _player;

    private void OnEnable()
    {
        if (_player == null)
        {
            _player = GameObject.FindWithTag("Player");
        }
        agent.BlackboardReference.SetVariableValue("Target", _player);
    }
}
