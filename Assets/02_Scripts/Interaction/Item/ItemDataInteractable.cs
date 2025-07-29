using UnityEngine;

/// <summary>
/// ItemData를 IInteractable처럼 행동하게 만들어주는 어댑터 클래스입니다.
/// </summary>
public class ItemDataInteractable : IInteractable
{
    private ItemData _itemData;

    // 생성 시 ItemData를 받아 저장합니다.
    public ItemDataInteractable(ItemData data)
    {
        _itemData = data;
    }

    // IInteractable 규칙에 따라 함수를 구현합니다.
    public string GetInteractPrompt()
    {
        return "F - 획득하기";
    }

    public void OnInteract()
    {
        // ItemManager를 찾아 아이템 사용 로직을 호출합니다.
        // 이 부분은 기존 PlayerInteractState에 있던 로직을 가져온 것입니다.
        Debug.Log($"--- 어댑터를 통해 {_itemData.itemName} 사용 ---");
        var itemManager = Object.FindObjectOfType<ItemManager>();
        if (itemManager != null)
        {
            itemManager.UseItem(_itemData);
        }
    }
}