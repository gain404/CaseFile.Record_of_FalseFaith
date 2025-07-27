using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIObjective : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private Transform objectiveContainer;// 생성된 퀘스트 항목들을 담는 컨테이너
    [SerializeField] private GameObject objectiveItemPrefab;// 퀘스트 항목 UI 프리팹
    [SerializeField] private TextMeshProUGUI chapterTitle;// 챕터 타이틀 텍스트
    [SerializeField] private ScrollRect scrollRect;
    [SerializeField] private GameObject targetPanel; // 토글할 패널을 따로 지정

    [Header("UI Settings")]
    [SerializeField] private float itemSpacing = 10f; // 퀘스트 항목 간 간격
    [SerializeField] private bool autoScroll = true;

    [Header("Toggle Settings")]
    [SerializeField] private KeyCode toggleKey = KeyCode.O; // UI 토글
    [SerializeField] private bool startVisible = false; // 시작 시 보이기 여부

    private Dictionary<int, ObjectiveUIItem> objectiveUIItems = new Dictionary<int, ObjectiveUIItem>();// UI에 표시할 목표들
    private VerticalLayoutGroup layoutGroup;


    private void Awake()
    {

        // targetPanel이 설정되지 않았다면 자식 패널을 찾거나 자기 자신을 사용
        if (targetPanel == null)
        {
            // 첫 번째 자식을 targetPanel로 사용 (보통 UI 패널 구조)
            if (transform.childCount > 0)
            {
                targetPanel = transform.GetChild(0).gameObject;
            }
            else
            {
                targetPanel = gameObject;
            }
        }
        // Layout Group 설정
        layoutGroup = objectiveContainer.GetComponent<VerticalLayoutGroup>();
        if (layoutGroup == null)
        {
            layoutGroup = objectiveContainer.gameObject.AddComponent<VerticalLayoutGroup>();
        }

        // 레이아웃 세부 설정 (인스펙터 창에서 해도 됨)
        layoutGroup.spacing = itemSpacing;
        layoutGroup.padding = new RectOffset(10, 10, 10, 10);
        layoutGroup.childAlignment = TextAnchor.UpperCenter;
        layoutGroup.childControlWidth = true;
        layoutGroup.childControlHeight = false;
        layoutGroup.childForceExpandWidth = true;
        layoutGroup.childForceExpandHeight = false;

        // Content Size Fitter 추가
        ContentSizeFitter sizeFitter = objectiveContainer.GetComponent<ContentSizeFitter>();
        if (sizeFitter == null)
        {
            sizeFitter = objectiveContainer.gameObject.AddComponent<ContentSizeFitter>();
        }
        sizeFitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;
    }

    private void Start()
    {
        // 초기 가시성 설정 - targetPanel만 비활성화
        if (targetPanel != null)
        {
            targetPanel.SetActive(startVisible);
        }
    }

    private void Update()
    {
        // 위에서 설정한 키로 토글
        if (Input.GetKeyDown(toggleKey))
        {
            ToggleObjectiveUI();
        }
    }

    // 퀘스트 UI 토글
    public void ToggleObjectiveUI()
    {
        if (targetPanel != null)
        {
            bool newState = !targetPanel.activeSelf;
            targetPanel.SetActive(newState);
        }
    }

    //안쓰긴 하는데 우선 만들어놓은 메서드
    public void ShowObjectiveUI()
    {
        gameObject.SetActive(true);
    }

    public void HideObjectiveUI()
    {
        gameObject.SetActive(false);
    }


    // 현재 UI가 열려 있는지 확인
    public bool IsVisible()
    {
        return gameObject.activeSelf;
    }


    public void UpdateObjectiveDisplay(List<ObjectiveData> objectives)
    {
        // 애니메이션 없이 즉시 업데이트 오류 없어지면 맨 밑에거 활성화해야함
        ClearObjectiveItems();

        foreach (var objective in objectives)
        {
            // 프리팹을 생성하여 컨테이너에 추가
            GameObject uiItem = Instantiate(objectiveItemPrefab, objectiveContainer);
            ObjectiveUIItem itemComponent = uiItem.GetComponent<ObjectiveUIItem>();
            // 목표 데이터 추가
            itemComponent.Setup(objective);
            // 딕셔너리에 추가 (진행도 업데이트용)
            objectiveUIItems[objective.idx] = itemComponent;
        }

        //StartCoroutine(UpdateObjectiveDisplayCoroutine(objectives));
    }


    private IEnumerator UpdateObjectiveDisplayCoroutine(List<ObjectiveData> objectives)
    {
        // 기존 UI 아이템 페이드아웃
        yield return StartCoroutine(FadeOutOldItems());

        // 기존 아이템 제거
        ClearObjectiveItems();

        // 새로운 목표 UI 생성
        for (int i = 0; i < objectives.Count; i++)
        {
            CreateObjectiveItem(objectives[i]);
            yield return new WaitForSeconds(0.1f); // 순차적으로 나타나는 효과
        }

        // 자동 스크롤
        if (autoScroll && scrollRect != null)
        {
            scrollRect.verticalNormalizedPosition = 1f;
        }
    }

    // 단일 목표 UI 생성
    private void CreateObjectiveItem(ObjectiveData objective)
    {
        GameObject uiItem = Instantiate(objectiveItemPrefab, objectiveContainer);
        ObjectiveUIItem itemComponent = uiItem.GetComponent<ObjectiveUIItem>();

        // 페이드인 애니메이션을 위한 초기 설정
        CanvasGroup canvasGroup = uiItem.GetComponent<CanvasGroup>();
        if (canvasGroup == null)
        {
            canvasGroup = uiItem.AddComponent<CanvasGroup>();
        }
        canvasGroup.alpha = 0f;

        itemComponent.Setup(objective);
        objectiveUIItems[objective.idx] = itemComponent;

        // 페이드인 애니메이션
        StartCoroutine(FadeInItem(canvasGroup));
    }

    // 목표 페이드인 효과
    private IEnumerator FadeInItem(CanvasGroup canvasGroup)
    {
        float timer = 0;
        while (timer < 0.3f)
        {
            timer += Time.deltaTime;
            canvasGroup.alpha = Mathf.Lerp(0f, 1f, timer / 0.3f);
            yield return null;
        }
        canvasGroup.alpha = 1f;
    }

    // 페이드아웃
    private IEnumerator FadeOutOldItems()
    {
        List<Coroutine> fadeCoroutines = new List<Coroutine>();

        foreach (var item in objectiveUIItems.Values)
        {
            if (item != null)
            {
                CanvasGroup canvasGroup = item.GetComponent<CanvasGroup>();
                if (canvasGroup != null)
                {
                    fadeCoroutines.Add(StartCoroutine(FadeOutItem(canvasGroup)));
                }
            }
        }

        // 모든 페이드아웃이 완료될 때까지 대기
        foreach (var coroutine in fadeCoroutines)
        {
            yield return coroutine;
        }
    }

    private IEnumerator FadeOutItem(CanvasGroup canvasGroup)
    {
        float timer = 0;
        while (timer < 0.2f)
        {
            timer += Time.deltaTime;
            canvasGroup.alpha = Mathf.Lerp(1f, 0f, timer / 0.2f);
            yield return null;
        }
        canvasGroup.alpha = 0f;
    }

    // UI 상에 있는 모든 목표 항목 제거
    private void ClearObjectiveItems()
    {
        foreach (Transform child in objectiveContainer)
        {
            Destroy(child.gameObject);
        }
        objectiveUIItems.Clear();
    }

    // 목표의 진행도를 UI에 반영 (예: 아이템을 3/5 획득)
    public void UpdateObjectiveProgress(ObjectiveData objective)
    {
        if (objectiveUIItems.ContainsKey(objective.idx))
        {
            objectiveUIItems[objective.idx].UpdateProgress(objective);
        }
    }

    // 목표를 완료 처리하여 UI에 완료 표시
    public void CompleteObjective(ObjectiveData objective)
    {
        if (objectiveUIItems.ContainsKey(objective.idx))
        {
            objectiveUIItems[objective.idx].MarkAsCompleted(true);
        }
    }

    public void SetChapterTitle(string title)
    {
        if (chapterTitle != null)
        {
            chapterTitle.text = title;
        }
    }

    // 디버그용 - 에디터에서 테스트할 수 있는 메서드
    [ContextMenu("Test Complete First Objective")]
    private void TestCompleteFirstObjective()
    {
        if (objectiveUIItems.Count > 0)
        {
            var firstItem = objectiveUIItems.Values.GetEnumerator();
            if (firstItem.MoveNext())
            {
                firstItem.Current.MarkAsCompleted(true);
            }
        }
    }
}
