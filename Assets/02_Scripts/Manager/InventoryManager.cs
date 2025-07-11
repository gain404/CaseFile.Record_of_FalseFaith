using System.Collections.Generic;
using UnityEngine;

public class InventoryManager : MonoBehaviour
{
    /// <summary>
    /// 다른 씬으로 이동해도 인벤토리를 유지해주는 싱글톤입니다.
    /// </summary>

    public static InventoryManager Instance;
    public ItemDatabase itemDatabase;
    public List<ItemData> items = new List<ItemData>();


    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created

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
                data.savedItemIDs.Add(item.name); // 또는 item.id 등 유니크한 식별자
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
                ItemData loadedItem = itemDatabase.GetItemByName(itemID); // 또는 GetItemByID
                if (loadedItem != null)
                    items.Add(loadedItem);
            }
        }
    }

}
