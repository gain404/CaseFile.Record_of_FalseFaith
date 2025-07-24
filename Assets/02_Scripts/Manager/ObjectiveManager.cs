using System.Collections.Generic;
using TMPro;
using UnityEngine;

[System.Serializable]
public class ObjectiveData
{
    public int idx;
    public string content;
    public bool achieve;
    public ObjectiveType type;
    public string targetId; // 아이템ID, 위치ID, 몬스터ID 등
    public int targetCount; // 필요한 개수
    public int currentCount; // 현재 진행도
}

public enum ObjectiveType
{
    CollectItem,    // 아이템 획득
    ReachLocation,  // 장소 도달
    DefeatMonster   // 몬스터 처치
}

public class ObjectiveManager : MonoBehaviour
{
    public static ObjectiveManager Instance { get; private set; }

    [SerializeField] private ObjectiveDataLoader dataLoader;
    [SerializeField] private UIObjective objectiveUI;

    private List<ObjectiveData> activeObjectives = new List<ObjectiveData>();
    private List<ObjectiveData> completedObjectives = new List<ObjectiveData>();

    public System.Action<ObjectiveData> OnObjectiveCompleted;
    public System.Action<ObjectiveData> OnObjectiveUpdated;

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

    private void Start()
    {
        dataLoader.LoadObjectiveData();
        LoadChapterObjectives(1); // 챕터 1 목표 로드 이건 테스트로 한 거고 어디에서 누가 대화한 후 퀘스트 추가 이러는 것도 가능
        objectiveUI.SetChapterTitle("Chapter 1");
        objectiveUI.UpdateObjectiveDisplay(activeObjectives);
    }

    public void LoadChapterObjectives(int chapterNumber)
    {
        activeObjectives.Clear();

        // null 체크 추가
        if (dataLoader == null)
        {
            Debug.LogError("DataLoader가 연결되지 않았습니다!");
            return;
        }

        // 챕터별 목표 로드 로직 (CSV에서 챕터 정보도 추가 가능)
        // 예시: 챕터 1의 목표들을 로드
        var objective1 = dataLoader.GetObjective(101);
        var objective2 = dataLoader.GetObjective(102);

        if (objective1 != null) activeObjectives.Add(objective1);
        if (objective2 != null) activeObjectives.Add(objective2);

        // objectiveUI null 체크 추가
        if (objectiveUI == null)
        {
            Debug.LogError("ObjectiveUI가 연결되지 않았습니다!");
            return;
        }

        objectiveUI.UpdateObjectiveDisplay(activeObjectives);
    }

    // 아이템 획득 시 호출
    public void OnItemCollected(string itemId, int count = 1)
    {
        // foreach 중에 리스트가 수정되는 것을 방지하기 위해 역순으로 반복
        for (int i = activeObjectives.Count - 1; i >= 0; i--)
        {
            var objective = activeObjectives[i];
            if (objective.type == ObjectiveType.CollectItem &&
                objective.targetId == itemId &&
                !objective.achieve)
            {
                objective.currentCount += count;

                if (objective.currentCount >= objective.targetCount)
                {
                    CompleteObjective(objective);
                }
                else
                {
                    OnObjectiveUpdated?.Invoke(objective);
                    objectiveUI.UpdateObjectiveProgress(objective);
                }
            }
        }
    }

    // 위치 도달 시 호출
    public void OnLocationReached(string locationId)
    {
        // 역순 반복으로 안전하게 처리
        for (int i = activeObjectives.Count - 1; i >= 0; i--)
        {
            var objective = activeObjectives[i];
            if (objective.type == ObjectiveType.ReachLocation &&
                objective.targetId == locationId &&
                !objective.achieve)
            {
                CompleteObjective(objective);
            }
        }
    }

    // 몬스터 처치 시 호출
    public void OnMonsterDefeated(string monsterId, int count = 1)
    {
        // 역순 반복으로 안전하게 처리
        for (int i = activeObjectives.Count - 1; i >= 0; i--)
        {
            var objective = activeObjectives[i];
            if (objective.type == ObjectiveType.DefeatMonster &&
                objective.targetId == monsterId &&
                !objective.achieve)
            {
                objective.currentCount += count;

                if (objective.currentCount >= objective.targetCount)
                {
                    CompleteObjective(objective);
                }
                else
                {
                    OnObjectiveUpdated?.Invoke(objective);
                    objectiveUI.UpdateObjectiveProgress(objective);
                }
            }
        }
    }

    private void CompleteObjective(ObjectiveData objective)
    {
        objective.achieve = true;
        completedObjectives.Add(objective);
        activeObjectives.Remove(objective);

        OnObjectiveCompleted?.Invoke(objective);
        objectiveUI.CompleteObjective(objective);

        Debug.Log($"목표 완료: {objective.content}");
    }

    public List<ObjectiveData> GetActiveObjectives()
    {
        return new List<ObjectiveData>(activeObjectives);
    }

    public bool IsChapterCompleted()
    {
        return activeObjectives.Count == 0;
    }
}

