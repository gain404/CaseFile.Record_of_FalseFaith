using UnityEngine;

[CreateAssetMenu(fileName = "New Heal Effect", menuName = "Item Effects/Direct Heal")]
public class HealEffectSO : ItemEffectSO
{
    [SerializeField] private StatType resourceType; // StatType.Hp 또는 StatType.Stamina
    [SerializeField] private float amount;

    public override void Apply(GameObject target)
    {
        var stats = target.GetComponent<StatManager>();
        if (stats == null) return;
        
        if(resourceType == StatType.Hp)
            stats.RecoverHp(amount);
        else if (resourceType == StatType.Stamina)
            stats.RecoverStamina(amount);
    }
}