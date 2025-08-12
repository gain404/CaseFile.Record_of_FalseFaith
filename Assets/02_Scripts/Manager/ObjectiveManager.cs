using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

[System.Serializable]
public class ObjectiveData
{
    public int idx; // 목표 고유 ID
    public string content; // 목표 설명 텍스트
    public bool achieve; // 목표 달성 여부
    public ObjectiveType type;


    // 여러 목표 항목
    public List<ObjectiveRequirement> requirements = new List<ObjectiveRequirement>();

    // 특정 퀘스트를 완료하면 이어서 활성화될 퀘스트들 [101;102] 이런 식으로 추가하시면 됩니다.
    public List<int> nextObjectiveIds = new List<int>();
}

[System.Serializable]
public class ObjectiveRequirement
{
    public string targetId;     // 아이템ID, 위치ID, 몬스터ID 등
    public int targetCount;     // 필요 개수
    public int currentCount;    // 현재 수집한 개수
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
    [SerializeField] private UIObjective objectiveUI; // 퀘스트 UI를 업데이트하는 UI 컴포넌트
    [SerializeField] private UIObjectiveCompleteNotifier completeNotifier; // 퀘스트 달성시 알려주는 UI 컴포넌트
    [SerializeField] private TriggerObjectiveDataLoader triggerLoader; // 아이템 먹을 시 트리거가 있는지 체크

    private List<ObjectiveData> activeObjectives = new List<ObjectiveData>(); // 현재 진행 중인 목표 목록
    private List<ObjectiveData> completedObjectives = new List<ObjectiveData>(); // 완료된 목표 목록


    // 목표가 완료되었을 때, 또는 업데이트 되었을 때 호출되는 이벤트
    public System.Action<ObjectiveData> OnObjectiveCompleted;
    public System.Action<ObjectiveData> OnObjectiveUpdated;


    private HashSet<int> registeredObjectiveIds = new HashSet<int>();

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

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        StartCoroutine(InitSceneLoaded());
    }

    private IEnumerator InitSceneLoaded()
    {
        yield return null;



        triggerLoader.LoadTriggerData();

        if (UIManager.Instance != null && UIManager.Instance.UIObjective != null)
        {
            objectiveUI = UIManager.Instance.UIObjective;
            completeNotifier = UIManager.Instance.UIObjectiveCompleteNotifier;
        }
        else
        {
            yield break;
        }

        dataLoader.LoadObjectiveData();// 전체 퀘스트 데이터 로드
        LoadChapterObjectives(GetChapterId());
        objectiveUI.SetChapterTitle($"Chapter {GetChapterId()}");
        objectiveUI.UpdateObjectiveDisplay(activeObjectives);// UI에 퀘스트 목록 표시
    }

    public static int GetChapterId()
    {
        string name = SceneManager.GetActiveScene().name;

        return name switch
        {
            "Tutorial" => 0,
            "Chapter1" => 1,
            "Chapter2" => 2,
            _ => -1,               // 미정/예외
        };
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

        if (chapterNumber == 0)
        {
            var objective0 = dataLoader.GetObjective(001);

            if (objective0 != null) activeObjectives.Add(objective0);
        }
        // 챕터별 목표 로드 로직 (CSV에서 챕터 정보도 추가 가능)
        // 예시: 챕터 1의 목표들을 로드
        if (chapterNumber == 1)
        {
            var objective1 = dataLoader.GetObjective(101);
            var objective2 = dataLoader.GetObjective(102);

            if (objective1 != null) activeObjectives.Add(objective1);
            if (objective2 != null) activeObjectives.Add(objective2);
        }
        if(chapterNumber == 2)
        {

        }




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

            if (objective.type == ObjectiveType.CollectItem && !objective.achieve) // 아이템 모으는 퀘스트이고 달성되지 않았다면
            {
                bool updated = false;

                foreach (var req in objective.requirements)
                {
                    if (req.targetId == itemId)
                    {
                        req.currentCount += count;
                        req.currentCount = Mathf.Min(req.currentCount, req.targetCount); // 현재 소지품과 목표 개수 중 적은 쪽으로 업데이트
                        updated = true;
                    }
                }
                // 위의 foreach문에서 변화가 있었다면 
                if (updated)
                {
                    // 전체 조건 달성 여부 체크
                    bool allMet = objective.requirements.TrueForAll(r => r.currentCount >= r.targetCount);
                    // 모두 충족되었다면
                    if (allMet)
                    {
                        CompleteObjective(objective);
                    }
                    else // 아니라면
                    {
                        OnObjectiveUpdated?.Invoke(objective);
                        objectiveUI.UpdateObjectiveProgress(objective);
                    }
                }
            }
        }

        // 추가: 아이템 기반 퀘스트 트리거
        var triggered = triggerLoader.GetTriggeredQuests(itemId);
        foreach (int questId in triggered)
        {
            if (!registeredObjectiveIds.Contains(questId))
            {
                var newObjective = dataLoader.GetObjective(questId);
                if (newObjective != null)
                {
                    activeObjectives.Add(newObjective);
                    registeredObjectiveIds.Add(questId);

                    objectiveUI.UpdateObjectiveDisplay(activeObjectives); // UI에도 반영
                    Debug.Log($"아이템 {itemId}로 인해 퀘스트 {questId}가 추가되었습니다.");
                }
            }
        }
    }

    // 위치 도달 시 호출
    public void OnLocationReached(string locationId)
    {
        Debug.Log("목표 도달 확인하기");
        // 역순 반복으로 안전하게 처리
        for (int i = activeObjectives.Count - 1; i >= 0; i--)
        {
            var objective = activeObjectives[i];


            if (objective.type == ObjectiveType.ReachLocation && !objective.achieve)
            {
                bool updated = false;
                foreach (var req in objective.requirements)
                {
                    Debug.Log($"{locationId}와 목표인 {req.targetId}가 같은지 확인");
                    if (req.targetId == locationId)
                    {
                        Debug.Log($"{locationId} 도달 퀘스트 완료");
                        req.currentCount = 1; // 도달은 한 번으로 완료되므로
                        updated = true;
                    }
                }

                if (updated)
                {
                    bool allMet = objective.requirements.TrueForAll(r => r.currentCount >= r.targetCount);
                    if (allMet)
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
    }

    // 몬스터 처치 시 호출
    public void OnMonsterDefeated(string monsterId, int count = 1)
    {
        // 역순 반복으로 안전하게 처리 : 순서대로 하면 중간에 수정되면서 오류 발생될 수도 있음
        for (int i = activeObjectives.Count - 1; i >= 0; i--)
        {
            var objective = activeObjectives[i];
            if (objective.type == ObjectiveType.DefeatMonster && !objective.achieve)
            {
                bool updated = false;

                foreach (var req in objective.requirements)
                {
                    if (req.targetId == monsterId)
                    {
                        req.currentCount += count;
                        req.currentCount = Mathf.Min(req.currentCount, req.targetCount);
                        updated = true;
                    }
                }

                if (updated)
                {
                    bool allMet = objective.requirements.TrueForAll(r => r.currentCount >= r.targetCount);
                    if (allMet)
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
    }


    //목표 달성시 완료 시켜주는 메서드
    private void CompleteObjective(ObjectiveData objective)
    {
        objective.achieve = true;// 달성 처리
        completedObjectives.Add(objective);// 완료 목록으로 이동
        activeObjectives.Remove(objective);// 진행 중 목록에서 제거

        OnObjectiveCompleted?.Invoke(objective);// 완료 이벤트 호출
        objectiveUI.CompleteObjective(objective);// UI에서 완료 처리

        completeNotifier?.ShowCompletedObjective(objective);

        Debug.Log($"목표 완료: {objective.content}");
    }

    // 현재 진행 중인 퀘스트 목록을 외부에서 참조할 수 있도록 반환
    public List<ObjectiveData> GetActiveObjectives()
    {
        return new List<ObjectiveData>(activeObjectives);// 복사본 반환
    }

    // 완료한 퀘스트 목록을 외부에서 참조할 수 있도록 반환
    public List<ObjectiveData> GetCompletedObjectives()
    {
        return new List<ObjectiveData>(completedObjectives);
    }

    // 현재 챕터의 모든 퀘스트가 완료되었는지 확인 - 우선 만들어놓음
    public bool IsChapterCompleted()
    {
        return activeObjectives.Count == 0;
    }


    //로드한 데이터를 UI에 적용하기 위한 메서드
    public void LoadObjectiveProgress(List<ObjectiveData> active, List<ObjectiveData> completed)
    {
        // 현재 리스트 초기화
        activeObjectives.Clear();
        completedObjectives.Clear();

        // 저장된 진행도 불러오기
        if (active != null)
            activeObjectives.AddRange(active);
        if (completed != null)
            completedObjectives.AddRange(completed);

        // UI 갱신
        objectiveUI.UpdateObjectiveDisplay(activeObjectives);
    }
}

