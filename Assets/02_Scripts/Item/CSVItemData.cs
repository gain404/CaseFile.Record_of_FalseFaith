using UnityEngine;


// 아이템 데이터 구조체 (CSV에서 로드할 거임)
[System.Serializable]
public class CSVItemData
{
    public int idx;
    public string itemName;
    public string itemDiscription;
    public string itemSpritePath;
    public int maxStack;
    public string itemType;
    public int value;
    public bool isConsumable;
    public bool canSearch;

    public CSVItemData(int idx, string itemName, string itemDiscription, string itemSpritePath, int maxStack, string itemType, int value, bool isConsumable, bool canSearch)
    {
        this.idx = idx;
        this.itemName = itemName;
        this.itemDiscription = itemDiscription;
        this.itemSpritePath = itemSpritePath;
        this.maxStack = maxStack;
        this.itemType = itemType;
        this.value = value;
        this.isConsumable = isConsumable;
        this.canSearch = canSearch;
    }
}

// 인벤토리 아이템 (ID + 수량로 저장)
[System.Serializable]
public class InventoryItem
{
    public int itemId;
    public int quantity;

    public InventoryItem(int id, int qty)
    {
        itemId = id;
        quantity = qty;
    }
}