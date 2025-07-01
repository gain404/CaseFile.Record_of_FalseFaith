using UnityEngine;

[CreateAssetMenu(fileName = "New Buff Effect", menuName = "Item Effects/Apply Buff")]
public class BuffEffectSO : ItemEffectSO
{
    [SerializeField] private Buff buffToApply;

    public override void Apply(GameObject target)
    {
        target.GetComponent<BuffManager>()?.ApplyBuff(buffToApply);
    }
}