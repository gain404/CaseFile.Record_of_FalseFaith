using System;
using DG.Tweening;
using Unity.Behavior;
using Unity.Cinemachine;
using Unity.VisualScripting;
using UnityEngine;

public class TestBossRoom : MonoBehaviour
{
    [SerializeField] private GameObject doorTilemap;
    [SerializeField] private LayerMask playerLayerMask;
    [SerializeField] private CinemachineCamera cineMachineCamera;
    [SerializeField] private GameObject monster;

    private BoxCollider2D _boxCollider2D;

    private void Awake()
    {
        _boxCollider2D = GetComponent<BoxCollider2D>();
        _boxCollider2D.enabled = true;
        doorTilemap.SetActive(false);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if ((playerLayerMask.value & (1 << other.gameObject.layer)) != 0)
        {
            doorTilemap.SetActive(true);
            _boxCollider2D.enabled = false;
            DOVirtual.DelayedCall(1.0f, () =>
            {
                cineMachineCamera.Priority = 25;
                
            });
        }
    }

    private void ActiveBoss()
    {
        string isInRoom = "IsPlayerInBossRoom";
        DOVirtual.DelayedCall(1.0f, () =>
        {
            BehaviorGraphAgent agent = monster.GetComponent<BehaviorGraphAgent>();
            monster.SetActive(true);
            agent.BlackboardReference.SetVariableValue(isInRoom, true);
        });
    }
}
