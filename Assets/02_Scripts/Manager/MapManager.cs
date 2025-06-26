using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;
using TMPro;

public class MapManager : MonoBehaviour
{
    [Header("Map UI Elements")]
    public GameObject mapCanvasRoot;           // 전체 맵 UI (비활성화용)
    public CanvasGroup mapCanvasGroup;         // 페이드용
    public GameObject backgroundDim;           // 어두운 배경
    public GameObject mapPanel;                // 지도와 버튼들
    public GameObject confirmPopup;            // 팝업창

    [Header("Popup Components")]
    public TextMeshProUGUI locationText;
    public Button yesButton;
    public Button noButton;

    [Header("Fade")]
    public CanvasGroup fadePanel;
    public float fadeDuration = 0.5f;
    public float mapFadeDuration = 0.3f;

    [Header("Map Toggle Control")]
    public Button mapToggleButton;             // 지도 열기/닫기 버튼
    
    [Header("Warning UI")]
    public GameObject warningPopup;
    public Button okButton;
    
    private string targetSceneName;
    private bool isMapOpen = false;
    private Coroutine mapFadeCoroutine;

    void Start()
    {
        // 초기화
        mapCanvasRoot.SetActive(false);
        mapCanvasGroup.alpha = 0f;
        confirmPopup.SetActive(false);
        fadePanel.alpha = 0f;
        fadePanel.blocksRaycasts = false;

        // 버튼 연결
        mapToggleButton.onClick.AddListener(ToggleMap);
        yesButton.onClick.AddListener(OnConfirmYes);
        noButton.onClick.AddListener(OnConfirmNo);
        okButton.onClick.AddListener(CloseWarningPopup); // ← 확인 버튼 기능 연결
        StartCoroutine(FadeIn());
    }

    public void ToggleMap()
    {
        if (isMapOpen)
            CloseMap();
        else
            OpenMap();
    }

    public void OpenMap()
    {
        if (mapFadeCoroutine != null) StopCoroutine(mapFadeCoroutine);
        mapCanvasRoot.SetActive(true);
        backgroundDim.SetActive(true);
        confirmPopup.SetActive(false);
        mapFadeCoroutine = StartCoroutine(FadeCanvasGroup(mapCanvasGroup, 0f, 1f, mapFadeDuration));
        isMapOpen = true;
    }

    public void CloseMap()
    {
        if (mapFadeCoroutine != null) StopCoroutine(mapFadeCoroutine);
        confirmPopup.SetActive(false);
        mapFadeCoroutine = StartCoroutine(FadeOutAndDisable(mapCanvasGroup, mapFadeDuration));
        backgroundDim.SetActive(false);
        isMapOpen = false;
    }

    public void OnLocationButtonClicked(string sceneName)
    {
        string currentScene = SceneManager.GetActiveScene().name;

        if (sceneName == currentScene)
        {
            // 현재 씬과 같으면 경고창 띄움
            warningPopup.SetActive(true);
            return;
        }

        // 다른 씬이면 기존대로 동작
        targetSceneName = sceneName;
        locationText.text = $"{sceneName}로 이동하시겠습니까?";
        confirmPopup.SetActive(true);
    }


    void OnConfirmYes()
    {
        confirmPopup.SetActive(false);
        StartCoroutine(FadeAndLoadScene(targetSceneName));
    }

    void OnConfirmNo()
    {
        confirmPopup.SetActive(false);
    }
    
    void CloseWarningPopup()
    {
        warningPopup.SetActive(false);
    }


    IEnumerator FadeCanvasGroup(CanvasGroup canvasGroup, float from, float to, float duration)
    {
        float t = 0f;
        canvasGroup.alpha = from;
        canvasGroup.interactable = false;
        canvasGroup.blocksRaycasts = false;

        while (t < duration)
        {
            t += Time.deltaTime;
            canvasGroup.alpha = Mathf.Lerp(from, to, t / duration);
            yield return null;
        }

        canvasGroup.alpha = to;
        canvasGroup.interactable = true;
        canvasGroup.blocksRaycasts = true;
    }

    IEnumerator FadeOutAndDisable(CanvasGroup canvasGroup, float duration)
    {
        float t = 0f;
        canvasGroup.interactable = false;
        canvasGroup.blocksRaycasts = false;

        while (t < duration)
        {
            t += Time.deltaTime;
            canvasGroup.alpha = Mathf.Lerp(1f, 0f, t / duration);
            yield return null;
        }

        canvasGroup.alpha = 0f;
        mapCanvasRoot.SetActive(false);
    }

    IEnumerator FadeAndLoadScene(string sceneName)
    {
        yield return StartCoroutine(FadeOut());
        SceneManager.LoadScene(sceneName);
    }

    IEnumerator FadeOut()
    {
        fadePanel.blocksRaycasts = true;
        float t = 0f;
        while (t < fadeDuration)
        {
            t += Time.deltaTime;
            fadePanel.alpha = Mathf.Lerp(0f, 1f, t / fadeDuration);
            yield return null;
        }
        fadePanel.alpha = 1f;
    }
    IEnumerator FadeIn()
    {
        fadePanel.alpha = 1f;
        fadePanel.blocksRaycasts = true;

        float t = 0f;
        while (t < fadeDuration)
        {
            t += Time.deltaTime;
            fadePanel.alpha = Mathf.Lerp(1f, 0f, t / fadeDuration);
            yield return null;
        }

        fadePanel.alpha = 0f;
        fadePanel.blocksRaycasts = false;
    }

}
