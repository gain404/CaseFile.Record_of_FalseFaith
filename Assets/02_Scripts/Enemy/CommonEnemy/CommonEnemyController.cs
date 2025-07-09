using System;
using System.Collections.Generic;
using Unity.Behavior;
using UnityEngine;

public class CommonEnemyController : MonoBehaviour
{
    [SerializeField] private PatrolPoint patrolPoint;
    [SerializeField] private List<float> floorPos;
    [SerializeField] private BehaviorGraphAgent agent;
    [SerializeField] private LayerMask hitLayerMask;

    private Transform _leftPoint;
    private Transform _rightPoint;
    private GameObject _player;

    private void OnEnable()
    {
        if (_player == null)
        {
            _player = GameObject.FindWithTag("Player");
        }
        agent.BlackboardReference.SetVariableValue("Target", _player);
        
        _leftPoint.position = new Vector3(patrolPoint.leftEnd, floorPos[patrolPoint.floor], patrolPoint.zPosition);
        _rightPoint.position = new Vector3(patrolPoint.rightEnd, floorPos[patrolPoint.floor], patrolPoint.zPosition);

        agent.BlackboardReference.SetVariableValue("PatrolZone1", _leftPoint);
        agent.BlackboardReference.SetVariableValue("PatrolZone2", _rightPoint);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (((1 << collision.gameObject.layer) & hitLayerMask) != 0)
        {
            agent.BlackboardReference.SetVariableValue("IsTargetDetected", true);
        }
    }
    
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (((1 << collision.gameObject.layer) & hitLayerMask) != 0)
        {
            agent.BlackboardReference.SetVariableValue("IsTargetDetected", false);
        }
    }
}
