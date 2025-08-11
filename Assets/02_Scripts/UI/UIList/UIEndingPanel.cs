using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIEndingPanel : MonoBehaviour
{
    [Header("Root")]
    [SerializeField] private GameObject root; // 엔딩 패널 전체(스크린샷의 Canvas 밑 Panel)

    [Header("Elements")]
    [SerializeField] private TMP_Text chapterText;         // "Chapter 1"
    [SerializeField] private TMP_Text clearText;           // "Clear"
    [SerializeField] private TMP_Text toBeContinuedText;   // "To be Continued"
    [SerializeField] private GameObject backButton;        // 타이틀로 돌아가기 버튼

    [Header("Timing")]
    [SerializeField] private float firstDelay = 0.5f; // 최초 지연
    [SerializeField] private float gap = 1.0f;        // 각 단계 간격

    [Header("Optional")]
    [SerializeField] private bool fadeInEach = false;   // true면 각 요소를 페이드 인
    [SerializeField] private float fadeInDuration = 0.25f;
    [SerializeField] private string titleSceneName = "Title";
    private UIFadePanel _uiFadePanel;
    private Coroutine _seq;

    private void Awake()
    {
        HideAll();
        if (root != null) root.SetActive(false);
    }

    private void Start() 
    {
        _uiFadePanel = UIManager.Instance.UIFadePanel;
    }
    // UIEndingPanel.cs
    public void ShowSequence()
    {
        var fade = UIManager.Instance?.UIFadePanel;
        
        if (fade != null)
            fade.transform.SetAsLastSibling();
        
        if (fade != null)
            fade.Fade(0.95f, 0.1f);
        
        if (root != null) root.SetActive(true);
        transform.SetAsLastSibling();

        HideAll();

        if (_seq != null) UIManager.Instance.StopCoroutine(_seq);
        _seq = UIManager.Instance.StartCoroutine(CoSequence());
    }



    private void HideAll()
    {
        if (chapterText) chapterText.gameObject.SetActive(false);
        if (clearText) clearText.gameObject.SetActive(false);
        if (toBeContinuedText) toBeContinuedText.gameObject.SetActive(false);
        if (backButton) backButton.SetActive(false);
    }

    private IEnumerator CoSequence()
    {
        yield return new WaitForSecondsRealtime(firstDelay);

        yield return ShowUI(chapterText);
        yield return new WaitForSecondsRealtime(gap);

        yield return ShowUI(clearText);
        yield return new WaitForSecondsRealtime(gap);

        yield return ShowUI(toBeContinuedText);
        yield return new WaitForSecondsRealtime(gap);

        if (backButton) backButton.SetActive(true);
        _seq = null;
    }

    private IEnumerator ShowUI(TMP_Text t)
    {
        if (!t) yield break;

        if (!fadeInEach)
        {
            t.gameObject.SetActive(true);
            yield break;
        }

        // 페이드 인 옵션
        var cg = t.GetComponent<CanvasGroup>();
        if (!cg) cg = t.gameObject.AddComponent<CanvasGroup>();
        t.gameObject.SetActive(true);
        cg.alpha = 0f;

        float tElapsed = 0f;
        while (tElapsed < fadeInDuration)
        {
            tElapsed += Time.unscaledDeltaTime;
            cg.alpha = Mathf.Clamp01(tElapsed / fadeInDuration);
            yield return null;
        }
        cg.alpha = 1f;
    }
    public void OnClick_BackToTitle()
    {
        StartCoroutine(FadeAndLoadTitle(titleSceneName));
    }
    private IEnumerator FadeAndLoadTitle(string sceneName)
    {
        _uiFadePanel.AllFade();
        _uiFadePanel.Fade(1f, 1f);
        yield return new WaitForSeconds(2f);
        // 로딩 씬 호출
        LoadingBar.LoadScene(sceneName);
    }
}
