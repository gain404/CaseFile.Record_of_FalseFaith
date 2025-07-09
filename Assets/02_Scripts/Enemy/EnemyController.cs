using System;
using Unity.Behavior;
using Unity.Behavior.GraphFramework;
using UnityEngine;

public class EnemyController : MonoBehaviour,IDamagable
{
    [SerializeField] private BehaviorGraphAgent agent;
    [SerializeField] private LayerMask mask;

    private float _enemyMaxHealth;
    private float _enemyCurrentHealth;
    private GameObject _player;
    
    private void Start()
    {
        agent.BlackboardReference.GetVariableValue<float>("MaxHealth", out _enemyMaxHealth);
        agent.BlackboardReference.GetVariableValue<float>("CurrentHealth", out _enemyCurrentHealth);
    }

    private void OnEnable()
    {
        if (_player == null)
        {
            _player = GameObject.FindWithTag("Player");
        }

        agent.BlackboardReference.SetVariableValue("Target", _player);
    }

    public void TakeDamage(float damage)
    {
        Debug.Log("보스 데미지 입음 : " + damage);
        _enemyCurrentHealth -= damage;
        agent.BlackboardReference.SetVariableValue("CurrentHealth", _enemyCurrentHealth);
        if (_enemyCurrentHealth <= 0)
        {
            Die();
        }
    }

    public void Die()
    {
        Debug.Log("보스 사망");
        //사망
    }
}
