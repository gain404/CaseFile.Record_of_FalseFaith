using System.Collections.Generic;
using System.IO;
using UnityEngine;

[System.Serializable]
public class InvestigationData
{
    public int chapter;
    public int index;
    public string name;
    public string content;
    public bool isOpen;
}

public class InvestigationCsvReader : MonoBehaviour
{
    private CsvManager _csvManager;
    private void Awake()
    {
        _csvManager = CsvManager.Instance;
        if(_csvManager.InvestigationData.Count == 0)
        {
            LoadCsv("InvestigationFileData");
        }
    }

    public void LoadCsv(string fileName)
    {
        TextAsset csvFile = Resources.Load<TextAsset>(fileName);
        if (csvFile == null)
        {
            Debug.LogError($"CSV 파일 {fileName}을 찾을 수 없습니다.");
            return;
        }

        using (StringReader reader = new StringReader(csvFile.text))
        {
            bool isFirstLine = true;
            while (reader.ReadLine() is { } line)
            {
                if (isFirstLine)
                {
                    isFirstLine = false;
                    continue; // 첫 줄 헤더는 무시
                }

                string[] parts = SplitCsvLine(line);

                if (parts.Length < 3) continue;
                if (!int.TryParse(parts[0], out int index))
                {
                    continue;
                }

                InvestigationData investigationData = new InvestigationData
                {
                    chapter = int.Parse(parts[0])/100,
                    index = int.Parse(parts[0]),
                    name = parts[1],
                    content = parts[2],
                    isOpen = false
                };
                _csvManager.InvestigationData.Add(investigationData.index,investigationData);
            }
        }
    }

    private string[] SplitCsvLine(string line)
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
