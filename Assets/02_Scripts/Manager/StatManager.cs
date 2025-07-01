using UnityEngine;
using System.Collections.Generic;
using System;

public class StatManager : MonoBehaviour
{
    [SerializeField] private CharacterData baseStatsTemplate;
    
    public Dictionary<StatType, float> CurrentStats { get; private set; }
    public float CurrentHp { get; private set; }
    public float MaxHp => GetStatValue(StatType.Hp);
    public float CurrentStamina { get; private set; }
    public float MaxStamina => GetStatValue(StatType.Stamina);

    public event Action OnStatsChanged;
    public event Action<float, float> OnHpChanged;
    public event Action<float, float> OnStaminaChanged;

    private void Awake()
    {
        var data = GetComponent<Player>().Data;
        CurrentStats = new Dictionary<StatType, float>();
        foreach (var stat in baseStatsTemplate.Stats)
        {
            CurrentStats.Add(stat.Type, stat.Value);
        }
        CurrentHp = MaxHp;
        CurrentStamina = MaxStamina;
    }

    public float GetStatValue(StatType type) => CurrentStats.TryGetValue(type, out float value) ? value : 0f;

    public void TakeDamage(float damage)
    {
        CurrentHp = Mathf.Max(CurrentHp - damage, 0);
        OnHpChanged?.Invoke(CurrentHp, MaxHp);
        if (CurrentHp <= 0) Die();
    }

    public void RecoverHp(float amount)
    {
        CurrentHp = Mathf.Min(CurrentHp + amount, MaxHp);
        OnHpChanged?.Invoke(CurrentHp, MaxHp);
    }
    
    public bool ConsumeStamina(float amount)
    {
        if (CurrentStamina < amount) return false;
        CurrentStamina = Mathf.Max(CurrentStamina - amount, 0);
        OnStaminaChanged?.Invoke(CurrentStamina, MaxStamina);
        return true;
    }
    
    public void RecoverStamina(float amount)
    {
        CurrentStamina = Mathf.Min(CurrentStamina + amount, MaxStamina);
        OnStaminaChanged?.Invoke(CurrentStamina, MaxStamina);
    }
    
    public void AddStatModifier(StatType type, float amount)
    {
        if (CurrentStats.ContainsKey(type))
        {
            CurrentStats[type] += amount;
            OnStatsChanged?.Invoke();
        }
    }

    public void RemoveStatModifier(StatType type, float amount)
    {
        if (CurrentStats.ContainsKey(type))
        {
            CurrentStats[type] -= amount;
            OnStatsChanged?.Invoke();
        }
    }

    private void Die()
    {
        Debug.Log($"{gameObject.name} has died.");
        // 사망 로직
    }
}