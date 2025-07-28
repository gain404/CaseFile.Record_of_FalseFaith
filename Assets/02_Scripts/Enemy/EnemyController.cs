using Unity.Behavior;
using UnityEngine;
using DG.Tweening;

public class EnemyController : MonoBehaviour,IDamagable
{
    [SerializeField] private BehaviorGraphAgent agent;
    [SerializeField] private LayerMask mask;
    [SerializeField] private SpriteRenderer spriteRenderer;
    
    private float _enemyCurrentHealth;
    private GameObject _player;
    private bool _isDie;

    private void Awake()
    {
        _player = GameObject.FindWithTag("Player");
    }
    
    private void Start()
    {
        agent.BlackboardReference.SetVariableValue("Target", _player);
        agent.BlackboardReference.GetVariableValue<float>("CurrentHealth", out _enemyCurrentHealth);
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
        agent.enabled = false;
        _isDie = true;
        spriteRenderer.DOFade(0, 2f).OnComplete(() => gameObject.SetActive(false));
    }
}
