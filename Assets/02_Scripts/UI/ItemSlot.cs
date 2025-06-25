using TMPro;
using UnityEngine;
using UnityEngine.UI;
/// <summary>
/// 인벤토리 UI 창에 보여질 아이템 슬롯 하나하나에 관한 스크립트입니다.
/// </summary>
public class ItemSlot : MonoBehaviour
{
    public ItemData item;   // 아이템 데이터

    public UIInventory inventory;
    public Image icon;
    public TextMeshProUGUI quatityText;  // 수량표시 Text
    private Outline outline;             // 선택시 Outline 표시위한 컴포넌트

    public int index;                    // 몇 번째 Slot인지 index 할당
    public int quantity;                 // 수량데이터

    private void Awake()
    {
        outline = GetComponent<Outline>();
    }


    // UI(슬롯 한 칸) 업데이트를 위한 함수
    // 아이템데이터에서 필요한 정보를 각 UI에 표시
    public void Set()
    {
        icon.gameObject.SetActive(true);
        icon.sprite = item.icon;
        quatityText.text = quantity > 1 ? quantity.ToString() : string.Empty;
    }

    // UI(슬롯 한 칸)에 정보가 없을 때 UI를 비워주는 함수
    public void Clear()
    {
        item = null;
        icon.gameObject.SetActive(false);
        quatityText.text = string.Empty;
    }

    // 슬롯을 클릭했을 때 발생하는 함수.
    public void OnClickButton()
    {
        // 인벤토리의 SelectItem 호출, 현재 슬롯의 인덱스만 전달.
        inventory.SelectItem(index);
    }
}
