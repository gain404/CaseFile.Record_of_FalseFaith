using TMPro;
using UnityEngine;

/// <summary>
/// 인벤토리UI를 다루는 스크립트입니다.
/// </summary>
public class UIInventory : MonoBehaviour
{
    public GameObject playerGameObject;
    private Player player;
    public ItemSlot[] slots; //인벤토리에 들어갈 아이템 슬롯들

    public GameObject inventoryWindow;
    public Transform slotPanel;
    public GameObject slotPrefab;

    [Header("Selected Item")]           // 선택한 슬롯의 아이템 정보 표시 위한 UI
    private ItemSlot selectedItem;
    private int selectedItemIndex;
    public TextMeshProUGUI selectedItemName;
    public TextMeshProUGUI selectedItemDescription;

    private PlayerController controller;
    private ItemManager itemUser;
    private int curEquipIndex;

    public UIAnimator inventoryAnimator;

    //나중에 플레이어 관련 정보도 추가되면 여기에 추가하기

    private void Start()
    {
        //플레이어 컨트롤러 등 여기에 초기화
        //controller = TestCharacterManager.Instance.Player.controller;
        controller = playerGameObject.GetComponent<PlayerController>();
        player = playerGameObject.GetComponent<Player>();

        // 플레이어 컨트롤러 등에서 Action 호출 시 필요한 함수 등록
        controller.inventory += Toggle;// inventory 키 입력 시
        itemUser = playerGameObject.GetComponent<ItemManager>();
        //TestCharacterManager.Instance.Player.addItem += AddItem;  // 아이템 획득 시
        player.addItem += AddItem;

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
        RefreshUI();
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
            inventoryAnimator.ClosePanel();
        }
        else
        {
            inventoryAnimator.OpenPanel();
        }
    }
    public bool IsOpen()
    {
        return inventoryWindow.activeInHierarchy;
    }


    public void AddItem()
    {
        //여기에 플레이어에게 추가되는 아이템 정보를 가져오게 하면 됩니다.
        ItemData data = TestCharacterManager.Instance.Player.itemData;

        //여러 개 소유 가능한 아이템일 경우
        if (data.canStack)
        {
            ItemSlot slot = GetItemStack(data);

            if(slot != null)
            {
                Debug.Log($"획득하려는 아이템 : {data.name}");
                slot.quantity++;
                UpdateUI();
                player.itemData = null;
                return;
            }
        }
        //위의 조건문을 돌지 않았다면 슬롯에 없다는 거니까 빈 슬롯을 찾기
        Debug.Log("빈 슬롯 찾아보겠습니다.");
        Debug.Log($"지금 슬롯 수:{slots.Length}");
        ItemSlot emptySlot = GetEmptySlot();
        if (emptySlot != null)
        {
            Debug.Log($"획득하려는 아이템 : {data.name}");
            emptySlot.item = data;
            emptySlot.quantity = 1;
            UpdateUI();
            player.itemData = null;
            return;
        }
        Debug.Log("인벤토리 부족");
        player.itemData = null;
    }

    // UI 정보 새로고침
    public void UpdateUI()
    {
        for (int i = 0; i < slots.Length; i++)
        {
            // 슬롯에 아이템 정보가 있다면
            if (slots[i].item != null)
            {
                Debug.Log($"{slots[i].item}의 정보를 세팅하겠습니다.");
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
        Debug.Log("빈 슬롯을 찾아보죠");
        for (int i = 0; i < slots.Length; i++)
        {
            Debug.Log($"{i}번째 슬롯입니다");
            if (slots[i].item == null)
            {
                Debug.Log($"오 찾음 : {i}번째 슬롯 비었음");
                return slots[i];
            }
            else
            {
                Debug.Log($"{i}번째 슬롯은 자리가 있습니다. 내용물 : {slots[i].item.name}");
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

    // UIInventory.cs

    public void UseItem()
    {
        if (selectedItem == null || selectedItem.item.type != ItemType.Consumable)
        {
            return;
        }
        
        bool success = itemUser.UseItem(selectedItem.item);
        
        if (success)
        {
            selectedItem.quantity--;

            if (selectedItem.quantity <= 0)
            {
                ClearSelectedItemWindow();
                slots[selectedItemIndex].Clear();
            }

            UpdateUI();
        }
    }

    public void RefreshUI()
    {
        // 기존 슬롯 삭제
        foreach (Transform child in slotPanel)
        {
            if (child.GetComponent<ItemSlot>().item == null)
            {
                //아이템 슬롯 안에 있는 아이템 데이타를 초기화 child = 판넬 안에 있는 slots
                Destroy(child.GetComponent<ItemSlot>().item);
            }
        }

        // 인벤토리 데이터 기반으로 다시 그림
        foreach (ItemData item in InventoryManager.Instance.items)
        {
            player.itemData = item;
            AddItem();
            //GameObject slot = Instantiate(slotPrefab, slotPanel);
            //slot.GetComponent<ItemSlot>().SetItem(item); // 슬롯에 정보 적용
        }
    }

}
