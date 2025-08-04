using System.Collections.Generic;
using UnityEngine;

public class ObjectiveDataLoader : MonoBehaviour
{
    [Header("CSV Files")]
    [SerializeField] private TextAsset goalDataCSV;
    [SerializeField] private TextAsset goalRequirmentCSV;

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
        if (goalDataCSV == null || goalRequirmentCSV == null)
        {
            Debug.LogError("CSV 파일이 설정되지 않았습니다!");
            return;
        }

        // 1단계: 퀘스트 본문 로드
        string[] questLines = goalDataCSV.text.Split('\n');

        for (int i = 1; i < questLines.Length; i++) // 헤더 스킵
        {
            string[] values = questLines[i].Trim().Split(',');
            if (values.Length < 3) continue;

            int idx = int.Parse(values[0]);
            string content = values[1];
            string typeStr = values[3];
            ObjectiveType type = ParseObjectiveType(typeStr);

            ObjectiveData data = new ObjectiveData
            {
                idx = idx,
                content = content,
                type = type,
                achieve = false,
                requirements = new List<ObjectiveRequirement>()
            };

            objectiveDatabase[idx] = data;
        }

        // 2단계: 조건 로드
        string[] reqLines = goalRequirmentCSV.text.Split('\n');

        for (int i = 1; i < reqLines.Length; i++)
        {
            string[] values = reqLines[i].Trim().Split(',');
            if (values.Length < 3) continue;

            int questIdx = int.Parse(values[0]);
            string targetId = values[1];
            int count = int.Parse(values[2]);

            if (objectiveDatabase.TryGetValue(questIdx, out var objective))
            {
                objective.requirements.Add(new ObjectiveRequirement
                {
                    targetId = targetId,
                    targetCount = count,
                    currentCount = 0
                });
            }
            else
            {
                Debug.LogWarning($"요구 조건에 대응되는 퀘스트 ID {questIdx}가 없습니다.");
            }
        }

        Debug.Log($"퀘스트 {objectiveDatabase.Count}개 로드 완료");
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
