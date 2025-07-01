using UnityEngine;

[System.Serializable]
public class Buff
{
    public string Name;
    public StatType TargetStat;
    public float Value;
    public float Duration;
    [HideInInspector] public float timeRemaining;

    public virtual void ApplyEffect(StatManager targetStats) => targetStats.AddStatModifier(TargetStat, Value);
    public virtual void RemoveEffect(StatManager targetStats) => targetStats.RemoveStatModifier(TargetStat, Value);
    public Buff Clone() => this.MemberwiseClone() as Buff;
}