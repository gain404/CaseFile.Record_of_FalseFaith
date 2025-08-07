using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using System;

public class UIShop : MonoBehaviour
{

    [Header("UI Panels")] 
    public GameObject ShopPanel;
    [SerializeField] private GameObject confirmationPopup;

    [Header("Item List (미리 생성된 슬롯)")]
    [SerializeField] private List<ShopItemSlot> itemSlots; // 프리팹 대신 미리 만든 슬롯 리스트를 받습니다.

    [Header("Item Details")]
    [SerializeField] private TMP_Text selectedItemName;
    [SerializeField] private TMP_Text selectedItemDescription;
    [SerializeField] private TMP_Text playerGoldText;

    [Header("Buttons & Popup")]
    [SerializeField] private Button buyButton;
    [SerializeField] private Button exitButton;
    [SerializeField] private TMP_Text confirmationText;
    [SerializeField] private Button confirmYesButton;
    [SerializeField] private Button confirmNoButton;

    private PlayerStat _playerStat;
    private ItemData _currentItemToBuy;
    private Player _player;
    private void Awake()
    {
        // 게임 시작 시 패널들을 비활성화합니다.
        if (ShopPanel != null) ShopPanel.SetActive(false);
        if (confirmationPopup != null) confirmationPopup.SetActive(false);
    }

    private void Start()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        _playerStat = player.GetComponent<PlayerStat>();
        buyButton.onClick.AddListener(OnBuyButtonPressed);
        exitButton.onClick.AddListener(CloseShop);
        confirmYesButton.onClick.AddListener(ConfirmPurchase);
        confirmNoButton.onClick.AddListener(() => confirmationPopup.SetActive(false));
    }

    public void OpenShop(ShopData data)
    {
        ShopPanel.SetActive(true);
        UpdatePlayerGold();
        PopulateShop(data.itemsForSale);
        ClearDetails();
    }

    public void CloseShop()
    {
        _player.stateMachine.IsReturningFromShop = true;

        // ✅ InteractState 강제 진입 유도 (또는 InteractUI → Interact 전이 조건 충족)
        _player.CurrentInteractableNPC = null;
        ShopPanel.SetActive(false);
    }
    
    void PopulateShop(List<ItemData> items)
    {
        // 미리 만들어 둔 슬롯의 개수만큼 반복합니다.
        for (int i = 0; i < itemSlots.Count; i++)
        {
            // 판매할 아이템이 슬롯보다 많거나 같으면
            if (i < items.Count)
            {
                // 슬롯을 활성화하고 데이터를 설정합니다.
                itemSlots[i].gameObject.SetActive(true);
                // ★ ShopItemSlot의 Setup에 UIShop 자기 자신(this)을 넘겨줍니다.
                itemSlots[i].Setup(items[i], this);
            }
            // 판매할 아이템이 더 이상 없으면
            else
            {
                // 남은 슬롯은 비활성화합니다.
                itemSlots[i].gameObject.SetActive(false);
            }
        }
    }
    
    public void SelectItem(ItemData item)
    {
        _currentItemToBuy = item;
        selectedItemName.text = item.itemName;
        selectedItemDescription.text = item.itemDescription;
    }

    void ClearDetails()
    {
        _currentItemToBuy = null;
        selectedItemName.text = "아이템 선택";
        selectedItemDescription.text = "구매할 아이템을 선택하세요.";
    }

    void UpdatePlayerGold()
    {
        if (_playerStat != null)
        {
            playerGoldText.text = _playerStat.GetStatValue(StatType.Money).ToString();
        }
    }
    
    void OnBuyButtonPressed()
    {
        if (_currentItemToBuy == null)
        {
            Debug.Log("구매할 아이템을 선택하세요.");
            return;
        }
        ShowConfirmationPopup($"'{_currentItemToBuy.itemName}'을(를) 구매하시겠습니까?", ConfirmPurchase);
    }

    void ConfirmPurchase()
    {
        if (_currentItemToBuy == null) return;

        if (_playerStat.Consume(StatType.Money, _currentItemToBuy.itemPrice))
        {
            UIManager.Instance.UIInventory.AddItem(_currentItemToBuy);
            Debug.Log($"{_currentItemToBuy.itemName} 구매 완료!");
        }
        else
        {
            Debug.Log("소지금이 부족합니다.");
        }
        UpdatePlayerGold();
        confirmationPopup.SetActive(false);
    }
    
    void ShowConfirmationPopup(string message, Action onConfirm)
    {
        confirmationPopup.SetActive(true);
        confirmationText.text = message;
        
        confirmYesButton.onClick.RemoveAllListeners();
        confirmNoButton.onClick.RemoveAllListeners();
        
        confirmYesButton.onClick.AddListener(() => { onConfirm?.Invoke(); });
        confirmNoButton.onClick.AddListener(() => confirmationPopup.SetActive(false));
    }
}