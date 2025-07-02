using Unity.Behavior;
using UnityEngine;

public class EnemyHealth : MonoBehaviour,IDamagable
{
    [SerializeField] private BehaviorGraphAgent agent;
    [SerializeField] private LayerMask mask;

    private float _enemyMaxHealth;
    private float _enemyCurrentHealth;
    
    private void Start()
    {
        agent.BlackboardReference.GetVariableValue<float>("MaxHealth", out _enemyMaxHealth);
        agent.BlackboardReference.GetVariableValue<float>("CurrentHealth", out _enemyCurrentHealth);
    }

    public void TakeDamage(float damage)
    {
        _enemyCurrentHealth -= damage;
        agent.BlackboardReference.SetVariableValue("CurrentHealth", _enemyCurrentHealth);
        if (_enemyCurrentHealth <= 0)
        {
            Die();
        }
    }

    public void Die()
    {
        //사망
    }
}
