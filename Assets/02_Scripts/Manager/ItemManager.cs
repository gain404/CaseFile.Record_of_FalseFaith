using UnityEngine;

public class ItemManager : MonoBehaviour
{
    public bool UseItem(ItemData item)
    {
        if (item != null && item.Effect != null)
        {
            Debug.Log($"Using {item.displayName}...");
            return item.Effect.Apply(this.gameObject);
        }
        else
        {
            Debug.LogWarning($"{item.displayName}(은)는 정의된 사용 효과가 없습니다.");
            return false; // 효과가 없으면 실패
        }
    }
}