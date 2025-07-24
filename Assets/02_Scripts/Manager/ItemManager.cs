using UnityEngine;

public class ItemManager : MonoBehaviour
{
    public bool UseItem(ItemData item)
    {
        if (item != null && item.itemType == ItemType.Recover)
        {
            return item.Effect.Apply(this.gameObject);
        }
        else
        {
            return false; // 효과가 없으면 실패
        }
    }
}