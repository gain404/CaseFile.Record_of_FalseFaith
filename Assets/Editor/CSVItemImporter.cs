using UnityEngine;
using UnityEditor;
using System.IO;
using System.Collections.Generic;

public class CSVItemImporter : MonoBehaviour
{
    [MenuItem("Tools/Import Items From CSV")]
    public static void ImportItems()
    {
        string csvPath = "Assets/Resources/itemData.csv"; // CSV 경로
        string[] lines = File.ReadAllLines(csvPath);

        for (int i = 1; i < lines.Length; i++) // 0번 줄은 헤더니까 스킵
        {
            string[] values = lines[i].Split(',');

            ItemData item = ScriptableObject.CreateInstance<ItemData>();
            item.idx = int.Parse(values[0]);
            item.itemName = values[1];
            item.itemDescription = values[2];
            item.itemType = (ItemType)System.Enum.Parse(typeof(ItemType), values[4]);
            item.healHP = int.Parse(values[5]);
            item.healStamina = int.Parse(values[6]);
            item.canSearch = values[7].ToLower() == "true";
            item.stageAvailable = int.Parse(values[8]);
            item.acquireCondition = values[9];
            item.itemPrice = int.Parse(values[10]);

            AssetDatabase.CreateAsset(item, $"Assets/Resources/Items/Item_{item.idx}.asset");
        }

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        Debug.Log("아이템 데이터 임포트 완료!");
    }
}
