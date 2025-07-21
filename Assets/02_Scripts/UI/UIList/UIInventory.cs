using TMPro;
using UnityEngine;

/// <summary>
/// 인벤토리UI를 다루는 스크립트입니다.
/// </summary>
public class UIInventory : MonoBehaviour
{
    [SerializeField] private ItemSlot[] slots; //인벤토리에 들어갈 아이템 슬롯들
    [SerializeField] private GameObject inventoryWindow;
    [SerializeField] private Transform slotPanel;
    [SerializeField] private GameObject slotPrefab;

    [Header("Selected Item")]           // 선택한 슬롯의 아이템 정보 표시 위한 UI
    [SerializeField] private TextMeshProUGUI selectedItemName;
    [SerializeField] private TextMeshProUGUI selectedItemDescription;
    [SerializeField] private UIAnimator inventoryAnimator;
    
    private ItemSlot _selectedItem;
    private ItemManager _itemUser;
    private Player _player;
    private PlayerController _playerController;
    private int _curEquipIndex;
    private int _selectedItemIndex;
    
    //나중에 플레이어 관련 정보도 추가되면 여기에 추가하기

    private void Start()
    {
        GameObject playerGameObject = GameObject.FindGameObjectWithTag("Player");
        _playerController = playerGameObject.GetComponent<PlayerController>();
        _player = playerGameObject.GetComponent<Player>();

        // 플레이어 컨트롤러 등에서 Action 호출 시 필요한 함수 등록
        _playerController.inventory += Toggle;// inventory 키 입력 시
        _itemUser = playerGameObject.GetComponent<ItemManager>();
        
        // 수정: 매개변수가 없는 _player.addItem 이벤트를 처리하기 위해 래퍼 함수를 연결합니다.
        _player.addItem += AddItemFromPlayerEvent;

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
        // RefreshUI(); // Start에서 호출 시 다른 매니저가 초기화되지 않았을 수 있으므로 주석 처리 권장
    }
    
    // 이 함수는 _player.addItem 이벤트가 발생했을 때 호출됩니다.
    private void AddItemFromPlayerEvent()
    {
        // Player 스크립트에 임시로 저장된 itemData를 가져옵니다.
        ItemData data = _player.itemData;
        if (data != null)
        {
            // 데이터가 있는 경우, 매개변수를 받는 AddItem 함수를 호출합니다.
            AddItem(data);
            _player.itemData = null; // 처리 후 비워줍니다.
        }
    }

    // 선택한 아이템 표시할 정보창 Clear 함수
    void ClearSelectedItemWindow()
    {
        _selectedItem = null;

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
    
    // 이제 이 함수는 상점에서든, 필드에서든 아이템을 추가하는 유일한 통로입니다.
    public void AddItem(ItemData data)
    {
        // 전달받은 데이터가 null이면 아무것도 하지 않습니다.
        if (data == null) return;

        //여러 개 소유 가능한 아이템일 경우
        if (data.canStack)
        {
            ItemSlot slot = GetItemStack(data);

            if(slot != null)
            {
                slot.quantity++;
                UpdateUI();
                return;
            }
        }

        //위의 조건문을 돌지 않았다면 슬롯에 없다는 거니까 빈 슬롯을 찾기
        ItemSlot emptySlot = GetEmptySlot();
        if (emptySlot != null)
        {
            emptySlot.item = data;
            emptySlot.quantity = 1;
            UpdateUI();
            return;
        }
        Debug.Log("인벤토리 부족");
    }

    // UI 정보 새로고침
    public void UpdateUI()
    {
        for (int i = 0; i < slots.Length; i++)
        {
            // 슬롯에 아이템 정보가 있다면
            if (slots[i].item != null)
            {
                //Debug.Log($"{slots[i].item}의 정보를 세팅하겠습니다.");
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

        _selectedItem = slots[index];
        _selectedItemIndex = index;

        selectedItemName.text = _selectedItem.item.displayName;
        selectedItemDescription.text = _selectedItem.item.description;
    }

    public void UseItem()
    {
        if (_selectedItem == null || _selectedItem.item.type != ItemType.Consumable)
        {
            return;
        }
        
        bool success = _itemUser.UseItem(_selectedItem.item);
        
        if (success)
        {
            _selectedItem.quantity--;

            if (_selectedItem.quantity <= 0)
            {
                ClearSelectedItemWindow();
                slots[_selectedItemIndex].Clear();
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
        // InventoryManager에서 아이템 정보를 가져와서, 매개변수가 있는 AddItem 함수를 직접 호출합니다.
        //if (InventoryManager.Instance != null)
        //{
        //    foreach (ItemData item in InventoryManager.Instance.items)
        //    {
        //        AddItem(item);
        //    }
        //}
    }

}