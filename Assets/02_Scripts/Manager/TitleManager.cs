using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class TitleManager : MonoBehaviour
{
    [Header("UI Panels")]
    public GameObject logoPanel;
    public GameObject mainMenuPanel;
    public GameObject savePanel;
    public GameObject settingsPanel; // 설정 패널 (기능은 아직 미구현!!)
    public GameObject confirmExitPopup;

    [Header("Fade Durations")]
    public float fadeDuration = 1.0f;
    public float logoStayDuration = 2.0f;
    
    [Header("Scene Transition")]
    public CanvasGroup fadePanel;
    
    void Start()
    {
        // 시작 시 모든 패널을 비활성화 (로고 패널 제외)
        mainMenuPanel.SetActive(false);
        savePanel.SetActive(false);
        settingsPanel.SetActive(false);
        confirmExitPopup.SetActive(false);
        
        if (fadePanel != null)
        {
            fadePanel.alpha = 0;
            fadePanel.blocksRaycasts = false;
        }

        // 타이틀 시퀀스 시작
        StartCoroutine(TitleSequence());
    }

    // 타이틀 씬의 전체 흐름을 관리하는 코루틴
    private IEnumerator TitleSequence()
    {
        yield return StartCoroutine(Fade(logoPanel, 0f, 1f, fadeDuration));
        
        yield return new WaitForSeconds(logoStayDuration);
        
        yield return StartCoroutine(Fade(logoPanel, 1f, 0f, fadeDuration));
        logoPanel.SetActive(false);
        
        mainMenuPanel.SetActive(true);
        yield return StartCoroutine(Fade(mainMenuPanel, 0f, 1f, fadeDuration));
    }
    
    
    public void OnClick_GameStart()
    {
        StartCoroutine(SwitchPanel(mainMenuPanel, savePanel));
    }
    
    public void OnClick_NewGameStart(string sceneName)
    {
        StartCoroutine(FadeAndLoadScene(sceneName));
    }
    
    public void OnClick_Settings()
    {
        // 현재는 설정 패널을 활성화
        // 추후 언어, 사운드 설정 UI를 연결하고 기능을 구현
        StartCoroutine(SwitchPanel(mainMenuPanel, settingsPanel));
        // 필요하다면 메인 메뉴를 비활성화하거나 페이드 아웃 처리
        // mainMenuPanel.SetActive(false);
    }
    
    public void OnClick_ExitGame()
    {
        confirmExitPopup.SetActive(true);
    }
    
    
    public void OnClick_ConfirmExit()
    {
        Debug.Log("게임을 종료합니다.");
        Application.Quit();
    }
    
    public void OnClick_CancelExit()
    {
        confirmExitPopup.SetActive(false);
    }
    
    
    public void OnClick_BackToMainMenu(GameObject currentPanel)
    {
        StartCoroutine(SwitchPanel(currentPanel, mainMenuPanel));
    }

    

    // 패널을 전환하는 코루틴
    private IEnumerator SwitchPanel(GameObject panelToFadeOut, GameObject panelToFadeIn)
    {
        yield return StartCoroutine(Fade(panelToFadeOut, 1f, 0f, fadeDuration));
        panelToFadeOut.SetActive(false);

        panelToFadeIn.SetActive(true);
        yield return StartCoroutine(Fade(panelToFadeIn, 0f, 1f, fadeDuration));
    }

    // UI 패널의 페이드 효과를 처리하는 범용 코루틴
    private IEnumerator Fade(GameObject panel, float startAlpha, float endAlpha, float duration)
    {
        CanvasGroup canvasGroup = panel.GetComponent<CanvasGroup>();
        if (canvasGroup == null)
        {
            canvasGroup = panel.AddComponent<CanvasGroup>();
        }

        float elapsedTime = 0f;
        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            canvasGroup.alpha = Mathf.Lerp(startAlpha, endAlpha, elapsedTime / duration);
            yield return null;
        }
        canvasGroup.alpha = endAlpha;
    }
    
    private IEnumerator FadeAndLoadScene(string sceneName)
    {
        if (fadePanel == null)
        {
            Debug.LogError("Fade Panel이 할당되지 않았습니다!");
            yield break;
        }
        
        // 페이드 아웃
        fadePanel.blocksRaycasts = true;
        float elapsedTime = 0f;
        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            fadePanel.alpha = Mathf.Lerp(0f, 1f, elapsedTime / fadeDuration);
            yield return null;
        }
        fadePanel.alpha = 1f;

        // 로딩 씬 호출
        LoadingBar.LoadScene(sceneName);
    }
}