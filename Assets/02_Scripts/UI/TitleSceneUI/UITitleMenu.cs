using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UITitleMenu : MonoBehaviour
{
    [Header("UI References")]
    public Canvas menuCanvas;
    public Image backgroundImage; // 배경 일러스트용
    public TextMeshProUGUI gameTitle;
    public Button[] menuButtons; // 3개 버튼
    public GameObject particleSystemPrefab;

    [Header("Button Settings")]
    public Color normalColor = new Color(0.16f, 0.16f, 0.24f, 1f);
    public Color hoverColor = new Color(0.23f, 0.23f, 0.31f, 1f);
    public Color glowColor = new Color(0f, 1f, 0.53f, 1f);

    [Header("Animation Settings")]
    public float hoverScale = 1.05f;
    public float hoverMoveX = 10f;
    public float animationDuration = 0.3f;

    [Header("Audio")]
    public AudioClip hoverSound;
    public AudioClip clickSound;

    private List<GameObject> particles = new List<GameObject>();
    private AudioSource audioSource;
    private bool isInitialized = false;

    [Header("Manager")]
    public TitleManager titleManager;

    void Start()
    {
        SetupUI();
        SetupButtons();
        SetupParticles();
        StartCoroutine(AnimateTitle());
        StartCoroutine(AnimateButtonsSequence());
    }

    void SetupUI()
    {
        // AudioSource 설정
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
            audioSource = gameObject.AddComponent<AudioSource>();

        // 배경 설정
        if (backgroundImage != null)
        {
            Color bgColor = backgroundImage.color;
            bgColor.a = 0.8f; // 80% 투명도
            backgroundImage.color = bgColor;
        }

        isInitialized = true;
    }

    void SetupButtons()
    {
        for (int i = 0; i < menuButtons.Length; i++)
        {
            Button button = menuButtons[i];
            int buttonIndex = i; // 클로저를 위한 로컬 복사

            // 초기 설정
            Image buttonImage = button.GetComponent<Image>();
            buttonImage.color = normalColor;

            // 초기 위치 설정 (화면 밖에서 시작)
            RectTransform rect = button.GetComponent<RectTransform>();
            rect.anchoredPosition = new Vector2(-300, rect.anchoredPosition.y);
            button.gameObject.SetActive(false);

            // 이벤트 트리거 추가
            EventTrigger trigger = button.gameObject.GetComponent<EventTrigger>();
            if (trigger == null)
                trigger = button.gameObject.AddComponent<EventTrigger>();

            // 호버 이벤트
            EventTrigger.Entry hoverEntry = new EventTrigger.Entry();
            hoverEntry.eventID = EventTriggerType.PointerEnter;
            hoverEntry.callback.AddListener((data) => { OnButtonHover(button); });
            trigger.triggers.Add(hoverEntry);

            // 호버 종료 이벤트
            EventTrigger.Entry exitEntry = new EventTrigger.Entry();
            exitEntry.eventID = EventTriggerType.PointerExit;
            exitEntry.callback.AddListener((data) => { OnButtonExit(button); });
            trigger.triggers.Add(exitEntry);

            // 클릭 이벤트
            EventTrigger.Entry clickEntry = new EventTrigger.Entry();
            clickEntry.eventID = EventTriggerType.PointerDown;
            clickEntry.callback.AddListener((data) => { OnButtonClick(button, data as PointerEventData); });
            trigger.triggers.Add(clickEntry);

            // 버튼별 클릭 액션 설정
            switch (buttonIndex)
            {
                case 0:
                    button.onClick.AddListener(StartGame);
                    break;
                case 1:
                    button.onClick.AddListener(ShowOptions);
                    break;
                case 2:
                    button.onClick.AddListener(EndGame);
                    break;
            }
        }
    }

    void OnButtonHover(Button button)
    {
        if (!isInitialized) return;

        SoundManager.Instance.PlaySFX(hoverSound);

        Image buttonImage = button.GetComponent<Image>();
        RectTransform rect = button.GetComponent<RectTransform>();
        TextMeshProUGUI text = button.GetComponentInChildren<TextMeshProUGUI>();

        // DOTween 애니메이션
        buttonImage.DOColor(hoverColor, animationDuration);
        rect.DOScale(hoverScale, animationDuration);
        rect.DOAnchorPosX(rect.anchoredPosition.x + hoverMoveX, animationDuration);

        if (text != null)
            text.DOColor(glowColor, animationDuration);

        // 글로우 효과 시뮬레이션
        StartCoroutine(GlowEffect(button));
    }

    void OnButtonExit(Button button)
    {
        if (!isInitialized) return;

        Image buttonImage = button.GetComponent<Image>();
        RectTransform rect = button.GetComponent<RectTransform>();
        TextMeshProUGUI text = button.GetComponentInChildren<TextMeshProUGUI>();

        // 원래 상태로 복귀
        buttonImage.DOColor(normalColor, animationDuration);
        rect.DOScale(1f, animationDuration);
        rect.DOAnchorPosX(rect.anchoredPosition.x - hoverMoveX, animationDuration);

        if (text != null)
            text.DOColor(Color.white, animationDuration);
    }

    void OnButtonClick(Button button, PointerEventData eventData)
    {
        if (!isInitialized) return;

        SoundManager.Instance.PlaySFX(clickSound);

        // 클릭 애니메이션
        RectTransform rect = button.GetComponent<RectTransform>();
        rect.DOScale(1.02f, 0.1f).OnComplete(() => {
            rect.DOScale(hoverScale, 0.1f);
        });

        // 리플 효과
        StartCoroutine(CreateRippleEffect(button, eventData.position));
    }

    IEnumerator CreateRippleEffect(Button button, Vector2 clickPosition)
    {
        // 리플 오브젝트 생성
        GameObject ripple = new GameObject("Ripple");
        ripple.transform.SetParent(button.transform, false);

        Image rippleImage = ripple.AddComponent<Image>();
        rippleImage.color = new Color(glowColor.r, glowColor.g, glowColor.b, 0.6f);
        rippleImage.raycastTarget = false;

        RectTransform rippleRect = ripple.GetComponent<RectTransform>();

        // 클릭 위치로 이동
        RectTransform buttonRect = button.GetComponent<RectTransform>();
        Vector2 localClickPos;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            buttonRect, clickPosition, null, out localClickPos);

        rippleRect.anchoredPosition = localClickPos;
        rippleRect.sizeDelta = Vector2.zero;

        // 원형 마스크 효과를 위한 설정
        rippleImage.sprite = CreateCircleSprite();

        // 애니메이션
        float maxSize = Mathf.Max(buttonRect.rect.width, buttonRect.rect.height) * 2;

        rippleRect.DOSizeDelta(new Vector2(maxSize, maxSize), 0.6f);
        rippleImage.DOFade(0f, 0.6f);

        yield return new WaitForSeconds(0.6f);

        if (ripple != null)
            DestroyImmediate(ripple);
    }

    Sprite CreateCircleSprite()
    {
        // 간단한 원형 스프라이트 생성 (또는 미리 만든 원형 스프라이트 사용)
        Texture2D texture = new Texture2D(100, 100);
        Color[] pixels = new Color[100 * 100];

        for (int x = 0; x < 100; x++)
        {
            for (int y = 0; y < 100; y++)
            {
                Vector2 center = new Vector2(50, 50);
                float distance = Vector2.Distance(new Vector2(x, y), center);

                if (distance <= 50)
                    pixels[y * 100 + x] = Color.white;
                else
                    pixels[y * 100 + x] = Color.clear;
            }
        }

        texture.SetPixels(pixels);
        texture.Apply();

        return Sprite.Create(texture, new Rect(0, 0, 100, 100), new Vector2(0.5f, 0.5f));
    }

    IEnumerator GlowEffect(Button button)
    {
        // 글로우 효과를 위한 추가 이미지 컴포넌트나 Outline 사용
        Outline outline = button.GetComponent<Outline>();
        if (outline == null)
            outline = button.gameObject.AddComponent<Outline>();

        outline.effectColor = glowColor;
        outline.effectDistance = new Vector2(2, 2);

        yield return new WaitForSeconds(0.3f);

        // 호버가 끝나면 제거됨 (OnButtonExit에서)
    }

    void SetupParticles()
    {
        // 파티클 시스템이 있다면 사용, 없다면 UI로 파티클 생성
        if (particleSystemPrefab != null)
        {
            GameObject particles = Instantiate(particleSystemPrefab, transform);
            particles.transform.SetAsFirstSibling(); // 배경에 배치
        }
        else
        {
            CreateUIParticles();
        }
    }

    void CreateUIParticles()
    {
        for (int i = 0; i < 30; i++)
        {
            GameObject particle = new GameObject($"Particle_{i}");
            particle.transform.SetParent(menuCanvas.transform, false);
            particle.transform.SetAsFirstSibling();

            Image particleImage = particle.AddComponent<Image>();
            particleImage.color = new Color(glowColor.r, glowColor.g, glowColor.b, 0.6f);
            particleImage.raycastTarget = false;

            RectTransform rect = particle.GetComponent<RectTransform>();
            rect.sizeDelta = new Vector2(4, 4);
            rect.anchoredPosition = new Vector2(
                Random.Range(-Screen.width / 2, Screen.width / 2),
                Random.Range(-Screen.height / 2, Screen.height / 2)
            );

            particles.Add(particle);

            // 파티클 애니메이션
            StartCoroutine(AnimateParticle(particle));
        }
    }

    IEnumerator AnimateParticle(GameObject particle)
    {
        RectTransform rect = particle.GetComponent<RectTransform>();
        Image image = particle.GetComponent<Image>();

        while (particle != null)
        {
            // 무작위 이동
            Vector2 startPos = rect.anchoredPosition;
            Vector2 endPos = startPos + new Vector2(
                Random.Range(-50f, 50f),
                Random.Range(-30f, 30f)
            );

            // 페이드 효과
            float startAlpha = Random.Range(0.3f, 0.8f);
            float endAlpha = Random.Range(0.1f, 0.5f);

            float duration = Random.Range(3f, 6f);
            float elapsed = 0;

            while (elapsed < duration && particle != null)
            {
                elapsed += Time.deltaTime;
                float t = elapsed / duration;

                rect.anchoredPosition = Vector2.Lerp(startPos, endPos, t);

                Color color = image.color;
                color.a = Mathf.Lerp(startAlpha, endAlpha, t);
                image.color = color;

                yield return null;
            }

            yield return new WaitForSeconds(Random.Range(0.5f, 2f));
        }
    }

    IEnumerator AnimateTitle()
    {
        if (gameTitle == null) yield break;

        while (true)
        {
            // 글로우 효과
            gameTitle.DOColor(Color.white, 1f).OnComplete(() => {
                gameTitle.DOColor(glowColor, 1f);
            });

            yield return new WaitForSeconds(2f);
        }
    }

    IEnumerator AnimateButtonsSequence()
    {
        yield return new WaitForSeconds(0.5f);

        for (int i = 0; i < menuButtons.Length; i++)
        {
            Button button = menuButtons[i];
            button.gameObject.SetActive(true);

            RectTransform rect = button.GetComponent<RectTransform>();

            // 슬라이드 인 애니메이션
            rect.DOAnchorPosX(60f, 0.8f).SetEase(Ease.OutBack);

            // 페이드 인
            CanvasGroup canvasGroup = button.GetComponent<CanvasGroup>();
            if (canvasGroup == null)
                canvasGroup = button.gameObject.AddComponent<CanvasGroup>();

            canvasGroup.alpha = 0;
            canvasGroup.DOFade(1f, 0.8f);

            yield return new WaitForSeconds(0.2f);
        }
    }

    // 버튼 액션들
    void StartGame()
    {
        Debug.Log("게임 시작");
        titleManager.OnClick_GameStart();
    }

    void ShowOptions()
    {
        Debug.Log("옵션");
        titleManager.OnClick_Settings();
    }

    void EndGame()
    {
        Debug.Log("게임 종료");
        titleManager.OnClick_ExitGame();
    }

    // 배경 이미지 변경 함수 (인스펙터나 다른 스크립트에서 호출)
    public void ChangeBackgroundImage(Sprite newSprite)
    {
        if (backgroundImage != null)
            backgroundImage.sprite = newSprite;
    }
}
