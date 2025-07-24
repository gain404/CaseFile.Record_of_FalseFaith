using System.Collections.Generic;
using UnityEngine;

public class ObjectiveDataLoader : MonoBehaviour
{
    [SerializeField] private TextAsset csvFile;
    private Dictionary<int, ObjectiveData> objectiveDatabase = new Dictionary<int, ObjectiveData>();

    private void Start()
    {
        // UIManager가 완전히 초기화된 후에 실행
        StartCoroutine(DelayedLoad());
    }

    private System.Collections.IEnumerator DelayedLoad()
    {
        // UIManager 초기화 대기
        while (UIManager.Instance == null || UIManager.Instance.UIHealth == null)
        {
            yield return null;
        }

        LoadObjectiveData();
    }

    public void LoadObjectiveData()
    {
        if (csvFile == null) return;

        string[] lines = csvFile.text.Split('\n');

        // 헤더 스킷
        for (int i = 1; i < lines.Length; i++)
        {
            string[] values = lines[i].Split(',');
            if (values.Length < 3) continue;

            ObjectiveData data = new ObjectiveData
            {
                idx = int.Parse(values[0]),
                content = values[1],
                achieve = bool.Parse(values[2]),
                type = ParseObjectiveType(values[3]),
                targetId = values.Length > 4 ? values[4] : "",
                targetCount = values.Length > 5 ? int.Parse(values[5]) : 1,
                currentCount = 0
            };

            objectiveDatabase[data.idx] = data;
        }
    }

    private ObjectiveType ParseObjectiveType(string typeString)
    {
        switch (typeString.ToLower())
        {
            case "item": return ObjectiveType.CollectItem;
            case "location": return ObjectiveType.ReachLocation;
            case "monster": return ObjectiveType.DefeatMonster;
            default: return ObjectiveType.CollectItem;
        }
    }

    public ObjectiveData GetObjective(int idx)
    {
        objectiveDatabase.TryGetValue(idx, out ObjectiveData data);
        if (data == null)
        {
            Debug.LogWarning($"ID {idx}에 해당하는 목표를 찾을 수 없습니다.");
        }
        return data;
    }

    // 디버그용 메서드 추가
    public void PrintAllObjectives()
    {
        Debug.Log("=== 로드된 모든 목표 ===");
        foreach (var kvp in objectiveDatabase)
        {
            var obj = kvp.Value;
            Debug.Log($"ID: {obj.idx}, 내용: {obj.content}, 타입: {obj.type}, 완료: {obj.achieve}");
        }
    }
}
