using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using TMPro;

public class MapManager : MonoBehaviour
{
    [Header("Core UI")]
    public GameObject mapCanvasRoot;    // 맵 UI 전체를 포함하는 루트 오브젝트
    public CanvasGroup mapCanvasGroup;  // 맵 UI의 페이드 처리용 CanvasGroup
    public Button mapToggleButton;      // 맵 UI(전체)를 여닫는 버튼

    [Header("Content Panels & Tabs")]
    public GameObject mapContentPanel;  // 맵 콘텐츠 패널
    public GameObject goalContentPanel; // 퀘스트(목표) 콘텐츠 패널
    public Button mapSwitchButton;      // 맵 콘텐츠로 전환하는 탭 버튼
    public Button goalSwitchButton;     // 퀘스트 콘텐츠로 전환하는 탭 버튼

    [Header("Content Fade")]
    public CanvasGroup contentCanvasGroup;    // 콘텐츠 패널의 페이드 처리용 CanvasGroup
    public float contentFadeDuration = 0.2f;  // 콘텐츠 페이드 지속 시간

    [Header("Map Open/Close Fade")]
    public float mapFadeDuration = 0.3f;      // 맵 전체 UI 페이드 지속 시간

    private bool isMapOpen = false;           // 현재 맵 UI가 열려 있는지 여부
    private bool isSwitching = false;         // 콘텐츠 전환 중인지 여부
    private Coroutine mapFadeCoroutine;       // 맵 UI 페이드 코루틴 참조
    private Coroutine switchContentCoroutine; // 콘텐츠 전환 코루틴 참조

    void Start()
    {
        // 맵 UI 비활성화 및 투명 처리
        mapCanvasRoot.SetActive(false);
        mapCanvasGroup.alpha = 0f;
        // 버튼에 클릭 이벤트 자동으로 연결
        mapToggleButton.onClick.AddListener(ToggleMap);
        mapSwitchButton.onClick.AddListener(() => SwitchToPanel(mapContentPanel));
        goalSwitchButton.onClick.AddListener(() => SwitchToPanel(goalContentPanel));
    }


    /// <summary>
    /// 맵 UI 열기/닫기 토글
    /// </summary>
    public void ToggleMap()
    {
        isMapOpen = !isMapOpen;
        if (isMapOpen)
            OpenMapUI();
        else
            CloseMapUI();
    }

    /// <summary>
    /// 맵 UI 열기 처리
    /// </summary>
    private void OpenMapUI()
    {
        // 기존 코루틴 중지
        if (mapFadeCoroutine != null) StopCoroutine(mapFadeCoroutine);
        // 먼저 맵을 보여주고 목표는 비활성화
        mapContentPanel.SetActive(true);
        goalContentPanel.SetActive(false);
        contentCanvasGroup.alpha = 1f;
        // 맵 버튼 비활성화, 퀘스트 버튼 활성화
        mapSwitchButton.interactable = false;
        goalSwitchButton.interactable = true;
        // 맵 UI 활성화 및 페이드 인
        mapCanvasRoot.SetActive(true);
        mapFadeCoroutine = StartCoroutine(FadeCanvasGroup(mapCanvasGroup, 0f, 1f, mapFadeDuration));
    }
    /// <summary>
    /// 전체 UI 닫기
    /// </summary>
    private void CloseMapUI()
    {
        if (mapFadeCoroutine != null) StopCoroutine(mapFadeCoroutine);
        mapFadeCoroutine = StartCoroutine(FadeOutAndDisable(mapCanvasGroup, mapFadeDuration));
    }
    /// <summary>
    /// 콘텐츠 패널 전환 시도
    /// </summary>
    private void SwitchToPanel(GameObject targetPanel)
    {
        // 이미 전환 중이거나 현재 패널과 같다면 무시
        if (isSwitching || targetPanel.activeSelf) return;

        if (switchContentCoroutine != null) StopCoroutine(switchContentCoroutine);
        switchContentCoroutine = StartCoroutine(SwitchContent(targetPanel));
    }

    /// <summary>
    /// 콘텐츠 전환 페이드 처리 코루틴
    /// </summary>
    IEnumerator SwitchContent(GameObject targetPanel)
    {
        isSwitching = true;
        // 현재 콘텐츠 페이드 아웃
        yield return StartCoroutine(FadeCanvasGroup(contentCanvasGroup, 1f, 0f, contentFadeDuration));
        // 대상 콘텐츠만 활성화
        mapContentPanel.SetActive(targetPanel == mapContentPanel);
        goalContentPanel.SetActive(targetPanel == goalContentPanel);
        // 버튼 상호작용 상태 변경
        mapSwitchButton.interactable = (targetPanel != mapContentPanel);
        goalSwitchButton.interactable = (targetPanel != goalContentPanel);
        // 새 콘텐츠 페이드 인
        yield return StartCoroutine(FadeCanvasGroup(contentCanvasGroup, 0f, 1f, contentFadeDuration));

        isSwitching = false;
    }
    /// <summary>
    /// CanvasGroup의 투명도를 페이드하는 코루틴
    /// </summary>
    IEnumerator FadeCanvasGroup(CanvasGroup canvasGroup, float from, float to, float duration)
    {
        float t = 0f;
        while (t < duration)
        {
            t += Time.deltaTime;
            canvasGroup.alpha = Mathf.Lerp(from, to, t / duration);
            yield return null;
        }
        canvasGroup.alpha = to;
    }
    /// <summary>
    /// 페이드 아웃 후 오브젝트 비활성화 처리
    /// </summary>
    IEnumerator FadeOutAndDisable(CanvasGroup canvasGroup, float duration)
    {
        yield return StartCoroutine(FadeCanvasGroup(canvasGroup, 1f, 0f, duration));
        mapCanvasRoot.SetActive(false);
    }
}