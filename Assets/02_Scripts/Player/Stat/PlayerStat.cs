// StatManager.cs

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using UnityEditor.U2D.Animation;

public class PlayerStat : MonoBehaviour
{
    public Dictionary<StatType, float> CurrentStats { get; private set; }
    private Dictionary<StatType, float> _maxStats;

    public event Action<StatType> OnStatChanged;

    public CharacterData characterData;//최대 체력 얻는 방식 변경 by 송도현

    public float CurrentHp => GetStatValue(StatType.Hp);
    public float MaxHp => GetMaxStatValue(StatType.Hp);

    [SerializeField] private HealthUI healthUI;//UI 표시 위한 추가
    [SerializeField] private StaminaUI staminaUI;

    public bool isInvincible;
    private bool isRecoveryOnCooldown = false;
    private const float RECOVERY_COOLDOWN_DURATION = 10.0f;
    

    private void Awake()
    {
        var data = GetComponent<Player>().Data;

        CurrentStats = new Dictionary<StatType, float>();
        _maxStats = new Dictionary<StatType, float>();

        foreach (var stat in data.Stats)
        {
            CurrentStats.Add(stat.Type, stat.Value);
            _maxStats.Add(stat.Type, stat.Value);
        }

        if (!CurrentStats.ContainsKey(StatType.Money))
        {
            CurrentStats.Add(StatType.Money, 0);
            _maxStats.Add(StatType.Money, float.MaxValue);
        }


    }

    public float GetStatValue(StatType type) => CurrentStats.TryGetValue(type, out float value) ? value : 0f;
    public float GetMaxStatValue(StatType type) => _maxStats.TryGetValue(type, out float value) ? value : 0f;

    public bool Consume(StatType type, float amount)
    {
        if (isInvincible)
            return false;
        
        if (amount <= 0)
        {
            Debug.LogWarning("소비량은 양수여야 합니다.");
            return false;
        }

        if (!CurrentStats.ContainsKey(type) || CurrentStats[type] < amount)
        {
            return false;
        }

        CurrentStats[type] -= amount;
        OnStatChanged?.Invoke(type);
        Debug.Log($"{type} {amount}만큼 소비. 현재: {CurrentStats[type]}");
        return true;
    }

    public bool UseRecoveryItem(StatType type, float amount)
    {
        if (amount <= 0)
        {
            Debug.Log("회복량이 유효하지 않습니다.");
            return false; // 아이템 사용 실패
        }

        if (GetStatValue(type) >= GetMaxStatValue(type))
        {
            Debug.Log($"{type}이(가) 이미 최대치라 사용할 수 없습니다.");
            return false; // 아이템 사용 실패
        }

        if (isRecoveryOnCooldown)
        {
            Debug.Log("재사용 대기 중입니다.");
            return false; // 아이템 사용 실패
        }

        // 모든 검사를 통과했으므로 성공
        Recover(type, amount);
        StartCoroutine(StartRecoveryCooldown());
    
        return true; // 아이템 사용 성공
    }

    private void Recover(StatType type, float amount)
    {
        if (amount <= 0) return;

        if (!CurrentStats.ContainsKey(type)) return;

        float maxStat = GetMaxStatValue(type);
        CurrentStats[type] = Mathf.Min(CurrentStats[type] + amount, maxStat);

        OnStatChanged?.Invoke(type);

        if(type == StatType.Hp)
        {
            healthUI.UpdateHearts((int)CurrentHp, (int)MaxHp);
        }
        else if (type == StatType.Stamina)
        {
            staminaUI.UpdateStamina(CurrentStats[type], maxStat);
        }

            Debug.Log($"{type}을(를) {amount} 만큼 회복. 현재 값: {CurrentStats[type]}");
    }

    private IEnumerator StartRecoveryCooldown()
    {
        isRecoveryOnCooldown = true;

        for (int i = (int)RECOVERY_COOLDOWN_DURATION; i > 0; i--)
        {
            Debug.Log($"회복 아이템 재사용까지... {i}초");
            yield return new WaitForSeconds(1.0f);
        }

        Debug.Log("회복 아이템 사용 가능!");
        isRecoveryOnCooldown = false;
    }

    public void TakeDamage(float damage)
    {
        if (Consume(StatType.Hp, damage))
        {
            if (CurrentStats[StatType.Hp] <= 0) Die();
        }
        healthUI.UpdateHearts((int)CurrentHp, (int)MaxHp);
    }

    private void Die()
    {
        Debug.Log($"{gameObject.name} 사망!");
    }
}