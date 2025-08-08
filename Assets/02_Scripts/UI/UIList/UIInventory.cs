using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>
/// 인벤토리UI를 다루는 스크립트입니다.
/// </summary>
public class UIInventory : MonoBehaviour
{
    [SerializeField] private ItemSlot[] slots; // 인벤토리에 들어갈 아이템 슬롯들
    [SerializeField] private GameObject inventoryWindow;
    [SerializeField] private Transform slotPanel;
    [SerializeField] private GameObject slotPrefab;

    [Header("Selected Item")] // 선택한 슬롯의 아이템 정보 표시 위한 UI
    [SerializeField] private TextMeshProUGUI selectedItemName;
    [SerializeField] private TextMeshProUGUI selectedItemDescription;
    [SerializeField] private UIAnimator inventoryAnimator;
    [SerializeField] private GameObject cancelButton;
    private ItemSlot _selectedItem;
    private ItemManager _itemUser;
    private Player _player;
    private PlayerController _playerController;
    private int _curEquipIndex;
    private int _selectedItemIndex;
    //  조사 모드 여부
    private bool _isInvestigationMode;

    private void Start()
    {
        GameObject playerGameObject = GameObject.FindGameObjectWithTag("Player");
        _playerController = playerGameObject.GetComponent<PlayerController>();
        _player = playerGameObject.GetComponent<Player>();

        // 플레이어 컨트롤러 등에서 Action 호출 시 필요한 함수 등록
        _playerController.inventory += Toggle; // inventory 키 입력 시
        _itemUser = playerGameObject.GetComponent<ItemManager>();
        
        // 수정: 매개변수가 없는 _player.addItem 이벤트를 처리하기 위해 래퍼 함수를 연결
        _player.addItem += AddItemFromPlayerEvent;

        // Inventory UI 초기화
        inventoryWindow.SetActive(false);
        slots = new ItemSlot[slotPanel.childCount];
        cancelButton.SetActive(false);
        for (int i = 0; i < slots.Length; i++)
        {
            slots[i] = slotPanel.GetChild(i).GetComponent<ItemSlot>();
            slots[i].index = i;
            slots[i].inventory = this;
            slots[i].Clear();
        }

        ClearSelectedItemWindow();
    }
    
    // _player.addItem 이벤트 발생 시 호출
    private void AddItemFromPlayerEvent()
    {
        ItemData data = _player.itemData;
        if (data != null)
        {
            AddItem(data);
            _player.itemData = null;
        }
    }

    // 선택한 아이템 정보창 초기화
    void ClearSelectedItemWindow()
    {
        _selectedItem = null;
        selectedItemName.text = string.Empty;
        selectedItemDescription.text = string.Empty;
    }

    // Inventory 창 Open/Close
    public void Toggle()
    {
        if (IsOpen())
        {
            inventoryAnimator.ClosePanel();
            ClearSelectedItemWindow();
            EventSystem.current.SetSelectedGameObject(null); // 선택 초기화
        }
        else
        {
            inventoryAnimator.OpenPanel();

            // 🔹 강제로 다시 Raycast/Selectable 업데이트 시도
            StartCoroutine(EnableUIInteractionsNextFrame());
        }
    }

    public bool IsOpen() => inventoryWindow.activeInHierarchy;
    
    // 아이템 추가
    public void AddItem(ItemData data)
    {
        if (data == null) return;

        // 여러 개 소유 가능한 아이템(=회복템)일 경우
        if (data.itemType == ItemType.Recover)
        {
            ItemSlot slot = GetItemStack(data);
            if(slot != null)
            {
                slot.quantity++;
                UpdateUI();
                return;
            }
        }

        // 빈 슬롯에 추가
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
            if (slots[i].item != null) slots[i].Set();
            else slots[i].Clear();
        }
    }

    // 여러 개 가질 수 있는 아이템의 정보 찾기
    ItemSlot GetItemStack(ItemData data)
    {
        for (int i = 0; i < slots.Length; i++)
        {
            if (slots[i].item == data && slots[i].quantity < data.maxStackAmount)
                return slots[i];
        }
        return null;
    }

    // 비어 있는 슬롯 찾기
    ItemSlot GetEmptySlot()
    {
        for (int i = 0; i < slots.Length; i++)
        {
            if (slots[i].item == null) return slots[i];
        }
        return null;
    }

    // 선택한 아이템 정보 표시
    public void SelectItem(int index)
    {
        if (slots[index].item == null) return;

        _selectedItem = slots[index];
        _selectedItemIndex = index;

        selectedItemName.text = _selectedItem.item.itemName;
        selectedItemDescription.text = _selectedItem.item.itemDescription;
    }

    // 아이템 사용
    public void UseItem()
    {
        if (_isInvestigationMode) return; // 조사 모드에서는 아이템 사용 금지

        if (_selectedItem == null || _selectedItem.item.itemType != ItemType.Recover)
            return;
        
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
    
    [SerializeField] private TextMeshProUGUI useButtonText; // 버튼 텍스트 연결

    public void EnterInvestigationMode()
    {
        
        _isInvestigationMode = true;
        if (!IsOpen()) Toggle();
        useButtonText.text = "조사";   // 조사 모드 표시
        cancelButton.SetActive(true);
        Debug.Log("[Inventory] 조사 모드 진입");
    }

    // In: UIInventory.cs

    public void ExitInvestigationMode()
    {
        Debug.Log("[Inventory] ExitInvestigationMode 메서드가 성공적으로 호출되었습니다."); // 확인용 로그 3
        _isInvestigationMode = false;
    
        Debug.Log($"[Inventory] 현재 인벤토리 활성화 상태(IsOpen): {IsOpen()}"); // 확인용 로그 4
        if (IsOpen())
        {
            Debug.Log("[Inventory] 인벤토리가 열려있어 Toggle()을 호출하여 닫습니다."); // 확인용 로그 5
            Toggle();
        }
        ClearSelectedItemWindow();
        useButtonText.text = "사용";
        cancelButton.SetActive(false);
        Debug.Log("[Inventory] 조사 모드 종료 로직 완료."); // 확인용 로그 6
    }


    public void InvestigateItem()
    {
        if (!_isInvestigationMode || _selectedItem == null) return;

        ItemData data = _selectedItem.item;
        if (!data.canInvestigate)
        {
            Debug.Log("[Inventory] 조사 불가능한 아이템입니다.");
            return;
        }

        // 1. 조사 타이머 시작
        UIInvestigationTimer.Instance.StartInvestigation(data.investigationIndex);

        // 2. 세컨드 대사 표시 허용
        _player.stateMachine.IsReturnFromInvestigationSuccess = true; //  조사 성공

        // 3. 인벤토리 닫기
        ExitInvestigationMode();
        // 4. 세컨드 대사 출력
        if (UIManager.Instance.UIDialogue != null)
        {
            UIManager.Instance.UIDialogue.ForceEndAndStartSecondDialogue();
        }
    }

    public void CancelInvestigation()
    {
        UIManager.Instance.UIDialogue.ResetDialogueState();
        _player.stateMachine.IsReturningFromInvestigationCancel = true;
        _player.stateMachine.IsReturnFromInvestigationSuccess = false; //  취소했으므로 false
        _isInvestigationMode = false;
        if (IsOpen()) Toggle();
        useButtonText.text = "사용";
        cancelButton.SetActive(false);
        ClearSelectedItemWindow();
    }




    //  인벤토리 새로고침 (요청하신 그대로 유지)
    public void RefreshUI()
    {
        Debug.Log("인벤토리 UI 새로고침하겠습니다.");
        // 기존 슬롯 삭제
        foreach (Transform child in slotPanel)
        {
            if (child.GetComponent<ItemSlot>().item == null)
            {
                // 아이템 슬롯 안의 아이템 데이터 초기화
                Destroy(child.GetComponent<ItemSlot>().item);
            }
        }
        
        // 인벤토리 아이템 기반으로 다시 그림
        if (InventoryManager.Instance != null)
        {
            Debug.Log("인벤토리 매니저로 새로고침하겠습니다.");
            foreach (InventoryItem item in InventoryManager.Instance.inventory)
            {
                for (int i = 0; i < item.quantity; i++)
                {
                    AddItemByIndex(item.itemId);
                }
            }
        }
    }
    
    private IEnumerator EnableUIInteractionsNextFrame()
    {
        yield return null; // 한 프레임 대기 (SetActive 이후)

        //  UI 이벤트 시스템 초기화
        EventSystem.current.SetSelectedGameObject(null);
    }

    public void AddItemByIndex(int idx)
    {
        string path = $"Item/Item_{idx}";
        ItemData item = Resources.Load<ItemData>(path);

        if (item != null)
        {
            AddItem(item);
            Debug.Log($"아이템 {item.itemName}을(를) 인벤토리에 추가했습니다.");
        }
        else
        {
            Debug.LogWarning($"아이템을 불러올 수 없습니다: {path}");
        }
    }
}
