using System;
using DG.Tweening;
using Unity.Behavior;
using Unity.Cinemachine;
using UnityEngine;

public class TestBossRoom : MonoBehaviour
{
    [SerializeField] private GameObject doorTilemap;
    [SerializeField] private LayerMask playerLayerMask;
    [SerializeField] private CinemachineCamera cineMachineCamera;
    [SerializeField] private GameObject monster;

    
    private void OnCollisionEnter2D(Collision2D other)
    {
        if ((playerLayerMask.value & (1 << other.gameObject.layer)) != 0)
        {
            doorTilemap.SetActive(true);
            this.gameObject.SetActive(false);
            DOVirtual.DelayedCall(1.0f, () =>
            {
                cineMachineCamera.Priority = 25;
                ActiveBoss();
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
