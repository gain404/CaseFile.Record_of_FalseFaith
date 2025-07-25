using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class CSVReader : MonoBehaviour
{
    [System.Serializable]
    public class DataEntry
    {
        public int idx;
        public string name;
        public string content;
    }

    public List<DataEntry> dataList = new();

    private void Start()
    {
        LoadCSV("data"); // Resources/data.csv 로 접근
    }

    public void LoadCSV(string fileName)
    {
        TextAsset csvFile = Resources.Load<TextAsset>(fileName);
        if (csvFile == null)
        {
            Debug.LogError($"CSV 파일 {fileName}을 찾을 수 없습니다.");
            return;
        }

        using (StringReader reader = new StringReader(csvFile.text))
        {
            string line;
            bool isFirstLine = true;
            while ((line = reader.ReadLine()) != null)
            {
                if (isFirstLine)
                {
                    isFirstLine = false;
                    continue; // 첫 줄 헤더는 무시
                }

                string[] parts = SplitCSVLine(line);
                if (parts.Length < 3) continue;

                DataEntry entry = new DataEntry
                {
                    idx = int.Parse(parts[0]),
                    name = parts[1],
                    content = parts[2]
                };

                dataList.Add(entry);
            }
        }

        Debug.Log($"총 {dataList.Count}개의 데이터를 불러왔습니다.");
    }

    private string[] SplitCSVLine(string line)
    {
        // 쉼표 안에 따옴표 포함된 항목도 처리
        List<string> result = new List<string>();
        bool inQuotes = false;
        string value = "";

        foreach (char c in line)
        {
            if (c == '\"')
            {
                inQuotes = !inQuotes;
            }
            else if (c == ',' && !inQuotes)
            {
                result.Add(value);
                value = "";
            }
            else
            {
                value += c;
            }
        }
        result.Add(value); // 마지막 항목 추가
        return result.ToArray();
    }
}
