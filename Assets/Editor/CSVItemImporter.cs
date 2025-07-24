using UnityEngine;
using UnityEditor;
using System.IO;
using System.Collections.Generic;
using NUnit.Framework.Interfaces;

public class CSVItemImporter : MonoBehaviour
{
    [MenuItem("Tools/Import Items From CSV")]
    public static void ImportItems()
    {
        string csvPath = "Assets/Resources/ItemData.csv"; // CSV 경로

        if (!File.Exists(csvPath))
        {
            Debug.LogError("CSV 파일을 찾을 수 없습니다: " + csvPath);
            return;
        }

        string[] lines = File.ReadAllLines(csvPath);

        // ScriptableObject를 저장할 폴더
        string assetFolder = "Assets/Resources/Item";
        if (!Directory.Exists(assetFolder))
            Directory.CreateDirectory(assetFolder);


        for (int i = 1; i < lines.Length; i++) // 0번 줄은 헤더니까 스킵
        {
            string[] values = lines[i].Split(',');

            if (values.Length < 11) continue; // 누락 방지

            ItemData item = ScriptableObject.CreateInstance<ItemData>();
            item.idx = int.Parse(values[0]);
            item.itemName = values[1];
            item.itemDescription = values[2];
            item.itemSprite = Resources.Load<Sprite>(values[3]);
            item.itemType = (ItemType)System.Enum.Parse(typeof(ItemType), values[4]);
            item.healHP = int.Parse(values[5]);
            item.healStamina = int.Parse(values[6]);
            item.canSearch = values[7].ToLower() == "true";//bool 값이라 이렇게
            item.stageAvailable = int.Parse(values[8]);
            item.acquireCondition = values[9];
            item.itemPrice = int.Parse(values[10]);
            item.maxStackAmount = int.Parse(values[11]);

            AssetDatabase.CreateAsset(item, $"Assets/Resources/Item/Item_{item.idx}.asset");
        }

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        Debug.Log("아이템 데이터 임포트 완료!");
    }
}
