using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using TMPro;

public class MapManager : MonoBehaviour
{
    [Header("Core UI")]
    public GameObject mapCanvasRoot;
    public CanvasGroup mapCanvasGroup;
    public Button mapToggleButton;

    [Header("Content Panels & Tabs")]
    public GameObject mapContentPanel;
    public GameObject goalContentPanel;
    public Button mapSwitchButton;
    public Button goalSwitchButton;

    [Header("Content Fade")]
    public CanvasGroup contentCanvasGroup;
    public float contentFadeDuration = 0.2f;

    [Header("Map Open/Close Fade")]
    public float mapFadeDuration = 0.3f;

    private bool isMapOpen = false;
    private bool isSwitching = false;
    private Coroutine mapFadeCoroutine;
    private Coroutine switchContentCoroutine;

    void Start()
    {
        mapCanvasRoot.SetActive(false);
        mapCanvasGroup.alpha = 0f;

        mapToggleButton.onClick.AddListener(ToggleMap);
        mapSwitchButton.onClick.AddListener(() => SwitchToPanel(mapContentPanel));
        goalSwitchButton.onClick.AddListener(() => SwitchToPanel(goalContentPanel));
    }

    public void ToggleMap()
    {
        isMapOpen = !isMapOpen;
        if (isMapOpen)
            OpenMapUI();
        else
            CloseMapUI();
    }

    private void OpenMapUI()
    {
        if (mapFadeCoroutine != null) StopCoroutine(mapFadeCoroutine);
        
        mapContentPanel.SetActive(true);
        goalContentPanel.SetActive(false);
        contentCanvasGroup.alpha = 1f;
        
        mapSwitchButton.interactable = false;
        goalSwitchButton.interactable = true;

        mapCanvasRoot.SetActive(true);
        mapFadeCoroutine = StartCoroutine(FadeCanvasGroup(mapCanvasGroup, 0f, 1f, mapFadeDuration));
    }

    private void CloseMapUI()
    {
        if (mapFadeCoroutine != null) StopCoroutine(mapFadeCoroutine);
        mapFadeCoroutine = StartCoroutine(FadeOutAndDisable(mapCanvasGroup, mapFadeDuration));
    }
    
    private void SwitchToPanel(GameObject targetPanel)
    {
        if (isSwitching || targetPanel.activeSelf) return;

        if (switchContentCoroutine != null) StopCoroutine(switchContentCoroutine);
        switchContentCoroutine = StartCoroutine(SwitchContent(targetPanel));
    }
    
    IEnumerator SwitchContent(GameObject targetPanel)
    {
        isSwitching = true;
        
        yield return StartCoroutine(FadeCanvasGroup(contentCanvasGroup, 1f, 0f, contentFadeDuration));
        
        mapContentPanel.SetActive(targetPanel == mapContentPanel);
        goalContentPanel.SetActive(targetPanel == goalContentPanel);
        
        mapSwitchButton.interactable = (targetPanel != mapContentPanel);
        goalSwitchButton.interactable = (targetPanel != goalContentPanel);
        
        yield return StartCoroutine(FadeCanvasGroup(contentCanvasGroup, 0f, 1f, contentFadeDuration));

        isSwitching = false;
    }
    
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
    
    IEnumerator FadeOutAndDisable(CanvasGroup canvasGroup, float duration)
    {
        yield return StartCoroutine(FadeCanvasGroup(canvasGroup, 1f, 0f, duration));
        mapCanvasRoot.SetActive(false);
    }
}