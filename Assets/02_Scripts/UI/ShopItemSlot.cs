using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ShopItemSlot : MonoBehaviour
{
    public Image itemImage;
    public TMP_Text itemName;
    public TMP_Text itemPrice;
    public Button itemButton;

    public void Setup(ItemData itemData, ShopManager manager)
    {
        if (itemData == null) return;
        
        if (itemImage != null) itemImage.sprite = itemData.icon;
        if (itemName != null) itemName.text = itemData.displayName;
        if (itemPrice != null) itemPrice.text = itemData.price.ToString();

        itemButton.onClick.RemoveAllListeners();
        // ★ 버튼을 누르면 ShopManager의 SelectItem 함수를 직접 호출합니다.
        itemButton.onClick.AddListener(() => manager.SelectItem(itemData));
    }
}