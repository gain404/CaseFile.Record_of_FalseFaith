using UnityEngine;

[CreateAssetMenu(fileName = "New Heal Effect", menuName = "Item Effects/Direct Heal")]
public class HealEffectSO : ItemEffectSO
{
    [SerializeField] private StatType statToRecover;
    [SerializeField] private float amount;
    
    public override bool Apply(GameObject target)
    {
        var stats = target.GetComponent<PlayerStat>();
        if (stats == null) return false; // StatManager가 없으면 실패

        // StatManager의 사용 결과를 그대로 반환
        return stats.UseRecoveryItem(statToRecover, amount);
    }
}