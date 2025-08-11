using UnityEditor;
using UnityEngine;

public class ItemManager : MonoBehaviour
{
    PlayerStat playerStat;
    public bool UseItem(ItemData item)
    {
        if (item != null && item.itemType == ItemType.Recover)
        {
            playerStat = FindAnyObjectByType<PlayerStat>();
            playerStat.Recover(StatType.Heart, (float) item.healHP);
            playerStat.Recover(StatType.Stamina, (float)item.healStamina);
            return true;
        }
        else
        {
            return false; // 효과가 없으면 실패
        }
    }
}