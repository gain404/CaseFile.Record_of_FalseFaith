using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class InventoryManager : Singleton<InventoryManager>
{

    [Header("인벤토리 설정")]
    public int maxSlots = 12;
    private List<InventoryItem> inventory = new List<InventoryItem>();

    //인벤토리에 아이템 추가하기

    public bool AddItem(int itemId, int quantity)
    {
        CSVItemData itemData = ItemDatabase.Instance.GetItemData(itemId);
        if(itemData == null)
        {
            Debug.LogError($"{itemId}는 존재하지 않는 아이템입니다.");
            return false;
        }

        //이미 가지고 있는 아이템인지 확인
        InventoryItem haveItem = inventory.Find(item => item.itemId == itemId);

        //없다면
        if(haveItem != null)
        {
            //스택 가능한 아이템이라면
            if (haveItem.quantity + quantity <= itemData.maxStack)
            {

            }
        }
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
