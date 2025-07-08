using Unity.Behavior;
using UnityEngine;

public class EnemyHealth : MonoBehaviour,IDamagable
{
    [SerializeField] private BehaviorGraphAgent agent;
    [SerializeField] private LayerMask mask;
    [SerializeField] private PolygonCollider2D meleeAttackCollider2D;
    [SerializeField] private PolygonCollider2D biteAttackCollider2D;

    private float _enemyMaxHealth;
    private float _enemyCurrentHealth;
    
    private void Start()
    {
        meleeAttackCollider2D.enabled = false;
        biteAttackCollider2D.enabled = false;
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

    private void MeleeAttack()
    {
        meleeAttackCollider2D.enabled = true;
    }

    private void MeleeAttackFinish()
    {
        meleeAttackCollider2D.enabled = false;
    }

    private void BiteAttack()
    {
        biteAttackCollider2D.enabled = true;
    }
    private void BiteAttackFinish()
    {
        biteAttackCollider2D.enabled = false;
    }
}
