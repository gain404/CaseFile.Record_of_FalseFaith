using System;
using System.Collections.Generic;
using UnityEngine;

public class ItemDatabase : MonoBehaviour
{
    public static ItemDatabase Instance;

    [Header("CSV 설정")]
    public TextAsset itemDataCSV; // Inspector에서 CSV 파일 할당

    private Dictionary<int, ItemData> itemDatabase = new Dictionary<int, ItemData>();

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            LoadItemDatabase();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void LoadItemDatabase()
    {
        if (itemDatabase == null)
        {
            Debug.LogError("itemDataCSV가 없습니다.");
            return;
        }
        //줄 바꿈 된 걸로 개별 아이템 구분
        string[] lines = itemDataCSV.text.Split("\n");
        //맨 처음은 목차니까 건너뜀
        for (int i = 1; i < lines.Length; i++)
        {
            string line = lines[i].Trim();
            if(string.IsNullOrEmpty(line) ) continue;

            string[] values = line.Split(",");
            //아이템 데이타에 들어가는 정보랑 values에 들어있는 정보랑 개수 비교
            if (values.Length >= 11)
            {
                try
                {
                    int idx = int.Parse(values[0]);
                    string itemName = values[1];
                    string itemDiscription = values[2];
                    string itemSprite = values[3];
                    ItemType itemType = (ItemType) Enum.Parse(typeof(ItemType), values[4]);
                    int healHP = int.Parse(values[5]);
                    int healStamina = int.Parse(values[6]);
                    bool canSearch = bool.Parse(values[7]);
                    int stageAvailable = int.Parse(values[8]);
                    string acquireCondition = values[9];
                    int itemPrice = int.Parse(values[10]);
                    int maxStackAmount = int.Parse(values[11]);


                    ItemData itemData = new ItemData(idx, itemName,  itemDiscription, itemSprite, itemType, healHP, healStamina, canSearch, stageAvailable, acquireCondition, itemPrice, maxStackAmount);
                    itemDatabase[idx] = itemData;
                    //CSVItemData csvItemData = new CSVItemData(idx, itemName, itemDiscription, itemSpritePath, maxStack, itemType, value, isConsumable, canSearch);
                    //itemDatabase[idx] = csvItemData;
                }
                catch (Exception ex)
                {
                    Debug.LogError($"csv 작업하다가 오류 발생 {i}번째 줄 : {ex.Message}");
                }
            }
        }
        Debug.Log($"아이템 데이터베이스 로드 완료: {itemDatabase.Count}개 아이템");
    }

    public ItemData GetItemData(int itemId)
    {
        itemDatabase.TryGetValue(itemId, out ItemData itemData);
        return itemData;
    }

    public Dictionary<int, ItemData> GetAllItems()
    {
        return itemDatabase;
    }
}
