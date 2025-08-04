using System;
using System.Collections.Generic;
using DG.Tweening;
using Unity.Behavior;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class TestBossRoom : MonoBehaviour
{
    [SerializeField] private List<GameObject> doors;
    [SerializeField] private LayerMask playerLayerMask;
    [SerializeField] private CinemachineCamera cineMachineCamera;
    [SerializeField] private GameObject monster;
    [SerializeField] private Light2D globalLight;

    private BoxCollider2D _boxCollider2D;

    private void Awake()
    {
        _boxCollider2D = GetComponent<BoxCollider2D>();
        _boxCollider2D.enabled = true;
        foreach (var  door in doors)
        {
            door.SetActive(false);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if ((playerLayerMask.value & (1 << other.gameObject.layer)) != 0)
        {
            foreach (var  door in doors)
            {
                door.SetActive(true);
            }
            _boxCollider2D.enabled = false;
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
            globalLight.color = new Color(0.01f, 0.01f, 0.01f, 1);
            BehaviorGraphAgent agent = monster.GetComponent<BehaviorGraphAgent>();
            monster.SetActive(true);
            agent.BlackboardReference.SetVariableValue(isInRoom, true);
        });
    }
}
