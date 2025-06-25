using TMPro;
using UnityEngine;

/// <summary>
/// 인벤토리UI를 다루는 스크립트입니다.
/// </summary>
public class UIInventory : MonoBehaviour
{
    public ItemSlot[] slots; //인벤토리에 들어갈 아이템 슬롯들

    public GameObject inventoryWindow;
    public Transform slotPanel;

    [Header("Selected Item")]           // 선택한 슬롯의 아이템 정보 표시 위한 UI
    private ItemSlot selectedItem;
    private int selectedItemIndex;
    public TextMeshProUGUI selectedItemName;
    public TextMeshProUGUI selectedItemDescription;

    private int curEquipIndex;

    //나중에 플레이어 관련 정보도 추가되면 여기에 추가하기

    private void Start()
    {
        //플레이어 컨트롤러 등 여기에 초기화



        // Inventory UI 초기화 로직들
        inventoryWindow.SetActive(false);
        slots = new ItemSlot[slotPanel.childCount];

        for (int i = 0; i < slots.Length; i++)
        {
            slots[i] = slotPanel.GetChild(i).GetComponent<ItemSlot>();
            slots[i].index = i;
            slots[i].inventory = this;
            slots[i].Clear();
        }

        ClearSelectedItemWindow();
    }

    // 선택한 아이템 표시할 정보창 Clear 함수
    void ClearSelectedItemWindow()
    {
        selectedItem = null;

        selectedItemName.text = string.Empty;
        selectedItemDescription.text = string.Empty;
    }


    // Inventory 창 Open/Close 시 호출
    public void Toggle()
    {
        if (IsOpen())
        {
            inventoryWindow.SetActive(false);
        }
        else
        {
            inventoryWindow.SetActive(true);
        }
    }
    public bool IsOpen()
    {
        return inventoryWindow.activeInHierarchy;
    }


    public void AddItem()
    {
        //여기에 플레이어에게 추가되는 아이템 정보를 가져오게 하면 됩니다.

        //여러 개 소유 가능한 아이템일 경우

        //빈 슬롯을 찾기
    }

    // UI 정보 새로고침
    public void UpdateUI()
    {
        for (int i = 0; i < slots.Length; i++)
        {
            // 슬롯에 아이템 정보가 있다면
            if (slots[i].item != null)
            {
                slots[i].Set();
            }
            else
            {
                slots[i].Clear();
            }
        }
    }

    // 여러개 가질 수 있는 아이템의 정보 찾아서 return
    ItemSlot GetItemStack(ItemData data)
    {
        for (int i = 0; i < slots.Length; i++)
        {
            if (slots[i].item == data && slots[i].quantity < data.maxStackAmount)
            {
                return slots[i];
            }
        }
        return null;
    }
    // 슬롯의 item 정보가 비어있는 정보 return
    ItemSlot GetEmptySlot()
    {
        for (int i = 0; i < slots.Length; i++)
        {
            if (slots[i].item == null)
            {
                return slots[i];
            }
        }
        return null;
    }

    // 선택한 아이템 정보창에 업데이트 해주는 함수
    public void SelectItem(int index)
    {
        if (slots[index].item == null) return;

        selectedItem = slots[index];
        selectedItemIndex = index;

        selectedItemName.text = selectedItem.item.displayName;
        selectedItemDescription.text = selectedItem.item.description;
    }

    public void UseItem()
    {

    }
}
