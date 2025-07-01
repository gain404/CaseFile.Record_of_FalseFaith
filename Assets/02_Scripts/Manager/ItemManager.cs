using UnityEngine;

public class ItemManager : MonoBehaviour
{
    // 인벤토리 UI(UIInventory)에서 이 함수를 호출합니다.
    public void UseItem(ItemData item)
    {
        // 아이템에 사용 효과가 있는지 확인
        if (item != null && item.Effect != null)
        {
            Debug.Log($"Using {item.displayName}...");
            // 아이템 데이터에 연결된 Effect의 Apply 함수를 호출하여 효과 발동!
            item.Effect.Apply(this.gameObject);
        }
        else
        {
            Debug.LogWarning($"{item.displayName}(은)는 정의된 사용 효과가 없습니다.");
        }
    }
}