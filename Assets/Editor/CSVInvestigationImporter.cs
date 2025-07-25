using UnityEngine;
using UnityEditor;
using System.IO;

public class CSVInvestigationImporter : MonoBehaviour
{
    [MenuItem("Tools/Import Investigation From CSV")]
    
    public static void ImportItems()
    {
        string csvPath = "Assets/Resources/InvestigationData.csv"; // CSV 경로

        if (!File.Exists(csvPath))
        {
            Debug.LogError("CSV 파일을 찾을 수 없습니다: " + csvPath);
            return;
        }

        string[] lines = File.ReadAllLines(csvPath);

        // ScriptableObject를 저장할 폴더
        string assetFolder = "Assets/ScriptableObjects/Investigation";
        if (!Directory.Exists(assetFolder))
            Directory.CreateDirectory(assetFolder);


        for (int i = 1; i < lines.Length; i++) // 0번 줄은 헤더니까 스킵
        {
            string[] values = lines[i].Split(',');

            InvestigationData data = ScriptableObject.CreateInstance<InvestigationData>();
            data.chapter = int.Parse(values[0]) / 100;
            data.indexNumber = int.Parse(values[0]);
            data.investigationName = values[1];
            data.investigationDescription = values[2];

            AssetDatabase.CreateAsset(data,
                $"Assets/ScriptableObjects/Investigation/InvestigationData_{data.indexNumber}.asset");
        }

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        Debug.Log("아이템 데이터 임포트 완료!");
    }
}
