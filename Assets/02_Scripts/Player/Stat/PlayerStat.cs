using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class PlayerStat : MonoBehaviour
{
    public bool isInvincible;
    //밑에 액션 안쓰면 지워주세요
    public event Action<StatType> OnStatChanged;
    public float CurrentHeart => GetStatValue(StatType.Heart);
    public float MaxHeart => GetMaxStatValue(StatType.Heart);
    public float GetStatValue(StatType type) => _currentStats.TryGetValue(type, out float value) ? value : 0f;
    public float GetMaxStatValue(StatType type) => _maxStats.TryGetValue(type, out float value) ? value : 0f;
    //일단 public으로 두긴하는데 그냥 private로 하고 함수 하나 만들어서 data전달하는 방식으로 하느게 좋을 것 같아요, 아니면 프로퍼티사용해서 private set하는게
    //public CharacterData characterData;
    
    private Dictionary<StatType, float> _currentStats;
    private Dictionary<StatType, float> _maxStats;
    private UIHealth _uiHealth;
    private UIStamina _uiStamina;
    private bool _isRecoveryOnCooldown;
    private const float RecoveryCooldownDuration = 10.0f;

    public GameOverManager gameOverManager; // 할당 필요

    private void Awake()
    {
        var data = GetComponent<Player>().Data;

        _currentStats = new Dictionary<StatType, float>();
        _maxStats = new Dictionary<StatType, float>();

        foreach (var stat in data.Stats)
        {
            _currentStats.Add(stat.Type, stat.Value);
            _maxStats.Add(stat.Type, stat.MaxValue);
        }

        if (!_currentStats.ContainsKey(StatType.Money))
        {
            _currentStats.Add(StatType.Money, 0);
            _maxStats.Add(StatType.Money, float.MaxValue);
        }
        _isRecoveryOnCooldown = false;
    }

    private void Start()
    {
        _uiHealth = UIManager.Instance.UIHealth;
        _uiStamina = UIManager.Instance.UIStamina;
        _uiHealth.UpdateHeart();
    }

    public bool Consume(StatType type, float amount)
    {
        if (isInvincible)
            return false;
        
        if (amount <= 0)
        {
            return false;
        }

        if (!_currentStats.ContainsKey(type) || _currentStats[type] < amount)
        {
            return false;
        }

        _currentStats[type] -= amount;
        OnStatChanged?.Invoke(type);
        return true;
    }

    public bool UseRecoveryItem(StatType type, float amount)
    {
        if (amount <= 0)
        {
            return false; // 아이템 사용 실패
        }

        if (GetStatValue(type) >= GetMaxStatValue(type))
        {
            return false; // 아이템 사용 실패
        }

        if (_isRecoveryOnCooldown)
        {
            return false; // 아이템 사용 실패
        }

        // 모든 검사를 통과했으므로 성공
        Recover(type, amount);
        StartCoroutine(StartRecoveryCooldown());
    
        return true; // 아이템 사용 성공
    }

    public void Recover(StatType type, float amount)
    {
        if (amount <= 0) return;

        if (!_currentStats.ContainsKey(type)) return;

        float maxStat = GetMaxStatValue(type);
        if (_currentStats[type] >= maxStat) return;

        _currentStats[type] = Mathf.Min(_currentStats[type] + amount, maxStat);

        OnStatChanged?.Invoke(type);

        if(type == StatType.Heart)
        {
            _uiHealth.UpdateHeart();
        }
        else if (type == StatType.Stamina)
        {
            _uiStamina.UpdateStamina(_currentStats[type], maxStat);
        }
    }

    private IEnumerator StartRecoveryCooldown()
    {
        _isRecoveryOnCooldown = true;

        for (int i = (int)RecoveryCooldownDuration; i > 0; i--)
        {
            yield return new WaitForSeconds(1.0f);
        }
        
        _isRecoveryOnCooldown = false;
    }

    public void AddStat(StatType type, float value)
    {
        float currentValue = _currentStats.GetValueOrDefault(type);
        float clampedValue = Mathf.Clamp(value + currentValue, 0, _maxStats[type]);

        _currentStats[type] = clampedValue;
    }
    
    public void TakeDamage(float damage)
    {
        if (Consume(StatType.Heart, damage))
        {
            if (_currentStats[StatType.Heart] <= 0) Die();
        }
        _uiHealth.UpdateHeart();
    }

    private void Die()
    {
        Debug.Log($"{gameObject.name} 사망!");
        // 연출부터 시작
        UIGameOverEffect.Instance.PlayYouDiedEffect();
        // 플레이어 비활성화, 애니메이션, 사운드 등도 여기에
    }
}