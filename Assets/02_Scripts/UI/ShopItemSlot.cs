using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ShopItemSlot : MonoBehaviour
{
    public Image itemImage;
    public Button itemButton;

    public void Setup(ItemData itemData, UIShop manager)
    {
        if (itemData == null) return;
        
        if (itemImage != null) itemImage.sprite = itemData.itemSprite;

        itemButton.onClick.RemoveAllListeners();
        // ★ 버튼을 누르면 ShopManager의 SelectItem 함수를 직접 호출합니다.
        itemButton.onClick.AddListener(() => manager.SelectItem(itemData));
    }
}