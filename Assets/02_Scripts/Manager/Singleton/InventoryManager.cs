using System.Collections.Generic;
using UnityEngine;

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

public class InventoryManager : Singleton<InventoryManager>
{

    [Header("인벤토리 설정")]
    public int maxSlots = 12;
    public List<InventoryItem> inventory = new List<InventoryItem>();

    //인벤토리에 아이템 추가하기

    public bool AddItem(int itemId, int quantity)
    {
        ItemData itemData = ItemDatabase.Instance.GetItemData(itemId);
        if (itemData == null)
        {
            Debug.LogError($"{itemId}는 존재하지 않는 아이템입니다.");
            return false;
        }

        //이미 가지고 있는 아이템인지 확인
        InventoryItem haveItem = inventory.Find(item => item.itemId == itemId);

        //있다면
        if (haveItem != null)
        {
            //스택 가능한 아이템이라면
            if (haveItem.quantity + quantity <= itemData.maxStackAmount)
            {
                haveItem.quantity += quantity;
                return true;
            }
            else
            {
                //개수가 초과하면 새로운 슬롯에 넣기
                // 스택 한계 초과 시 새 슬롯에 추가
                int remainingQuantity = (haveItem.quantity + quantity) - itemData.maxStackAmount;
                haveItem.quantity = itemData.maxStackAmount;

                if (inventory.Count < maxSlots)
                {
                    inventory.Add(new InventoryItem(itemId, remainingQuantity));
                    return true;
                }
                else
                {
                    Debug.LogWarning("인벤토리가 가득 참!");
                    return false;
                }
            }
        }
        else
        {
            // 새 아이템 추가
            if (inventory.Count < maxSlots)
            {
                inventory.Add(new InventoryItem(itemId, quantity));
                return true;
            }
            else
            {
                Debug.LogWarning("인벤토리가 가득 참!");
                return false;
            }
        }
    }

    // 아이템 제거
    public bool RemoveItem(int itemId, int quantity)
    {
        //제거할 아이템 찾음
        InventoryItem item = inventory.Find(invItem => invItem.itemId == itemId);

        //있으면 빼려는 개수만큼 있는지 확인
        if (item != null && item.quantity >= quantity)
        {
            item.quantity -= quantity;
            if (item.quantity <= 0)
                inventory.Remove(item);
            return true;
        }

        return false;
    }


    // 아이템 수량 확인
    public int GetItemQuantity(int itemId)
    {
        //우선 0으로 초기화
        int totalQuantity = 0;
        foreach (var item in inventory)
        {
            if (item.itemId == itemId)
            {
                totalQuantity += item.quantity;
            }
        }
        return totalQuantity;
    }

    // 인벤토리 데이터 가져오기 (세이브용)
    public List<InventoryItem> GetInventoryData()
    {
        return new List<InventoryItem>(inventory);
    }

    // 인벤토리 데이터 설정 (로드용)
    public void SetInventoryData(List<InventoryItem> inventoryData)
    {
        inventory = new List<InventoryItem>(inventoryData);
    }



    // 인벤토리 초기화
    public void ClearInventory()
    {
        inventory.Clear();
    }


    //기존에 있던 스크립트 - csv 파일로 대체하면서 우선 주석 처리
    /*
    public ItemDatabase itemDatabase;
    public List<ItemData> items = new List<ItemData>();


    public void AddItem(ItemData item)
    {
        if (item.type == ItemType.Resource)
        {
            //이 아이템은 씬에 종속되므로 바로 저장 필요 없음
            items.Add(item);
        }
        else if (item.type == ItemType.KeyItem || item.type == ItemType.Consumable)
        {
            // 중요/소비 아이템은 씬이 바뀌어도 유지되므로 영구 인벤토리에 저장
            items.Add(item);
            SaveInventory();
        }
    }

    public void RemoveItem(ItemData item)
    {
        items.Remove(item);
    }

    public void OnSceneChanged()
    {
        // 단서 아이템만 제거
        items.RemoveAll(item => item.type == ItemType.Resource);
    }

    //저장할 아이템만 세이브 데이터로 넘겨줌
    //Resource 얘네들은 씬 넘어가면 자동으로 없어지니까 저장하지 않음
    public void SaveInventory()
    {
        SaveData data = new SaveData();
        foreach (var item in items)
        {
            if (item.type == ItemType.KeyItem || item.type == ItemType.Consumable)
            {
                if (item.displayName != null)
                {
                    data.savedItemIDs.Add(item.displayName); // 또는 item.id 등 유니크한 식별자
                }
            }
        }

        string json = JsonUtility.ToJson(data);
        PlayerPrefs.SetString("InventoryData", json);
        PlayerPrefs.Save();
    }

    public void LoadInventory()
    {
        if (!PlayerPrefs.HasKey("InventoryData")) return;

        if (PlayerPrefs.HasKey("InventoryData"))
        {
            string json = PlayerPrefs.GetString("InventoryData");
            SaveData data = JsonUtility.FromJson<SaveData>(json);

            items.Clear();
            foreach (string itemID in data.savedItemIDs)
            {
                CSVItemData loadedItem = itemDatabase.GetItemByName(itemID); // 또는 GetItemByID
                if (loadedItem != null)
                    items.Add(loadedItem);
            }
        }
    }
    */

}
