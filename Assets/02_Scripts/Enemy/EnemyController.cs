using Unity.Behavior;
using UnityEngine;
using Action = System.Action;

public class EnemyController : MonoBehaviour,IDamagable
{
    public Action DieAction;
    
    [SerializeField] private BehaviorGraphAgent agent;
    [SerializeField] private LayerMask mask;
    
    private float _enemyCurrentHealth;
    private GameObject _player;
    private bool _isHide;

    private void Awake()
    {
        _player = GameObject.FindWithTag("Player");
        _isHide = false;
    }
    
    private void Start()
    {
        agent.BlackboardReference.SetVariableValue("Target", _player);
        agent.BlackboardReference.GetVariableValue<float>("CurrentHealth", out _enemyCurrentHealth);
        DieAction += Die;
    }

    public void TakeDamage(float damage)
    {
        if (_isHide)
        {
            return;
        }

        Debug.Log("몬스터 : -" + damage);
        _enemyCurrentHealth -= damage;
        agent.BlackboardReference.SetVariableValue("CurrentHealth", _enemyCurrentHealth);
        if (_enemyCurrentHealth <= 0)
        {
            DieAction();
        }
    }

    public void Die()
    {
        agent.enabled = false;
    }

    private void Hide()
    {
        _isHide = true;
    }

    private void Reveal()
    {
        _isHide = false;
    }
}
