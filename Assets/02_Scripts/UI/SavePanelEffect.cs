using DG.Tweening;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SavePanelEffect : MonoBehaviour
{
    [Header("Animation Settings")]
    [Tooltip("버튼들이 순서대로 등장하는 간격 (초)")]
    public float sequentialDelay = 0.2f;

    [Tooltip("개별 버튼 등장 애니메이션 시간 (초)")]
    public float buttonAppearDuration = 0.8f;

    [Tooltip("호버/클릭 애니메이션 시간 (초)")]
    public float hoverAnimationDuration = 0.3f;

    [Header("Hover Effects")]
    [Tooltip("호버 시 버튼 크기 배율")]
    public float hoverScale = 1.05f;

    [Tooltip("호버 시 버튼이 오른쪽으로 이동하는 거리")]
    public float hoverMoveX = 10f;

    [Header("Colors")]
    [Tooltip("기본 버튼 색상")]
    public Color normalColor = new Color(0.16f, 0.16f, 0.24f, 1f);

    [Tooltip("호버 시 버튼 색상")]
    public Color hoverColor = new Color(0.23f, 0.23f, 0.31f, 1f);

    [Tooltip("글로우/액센트 색상")]
    public Color glowColor = new Color(0f, 1f, 0.53f, 1f);

    [Header("Audio")]
    [Tooltip("마우스 호버 시 재생할 사운드")]
    public AudioClip hoverSound;

    [Tooltip("클릭 시 재생할 사운드")]
    public AudioClip clickSound;

    [Header("References")]
    [Tooltip("오디오 재생용 AudioSource (null이면 자동 생성)")]
    public AudioSource audioSource;

    // 내부 변수들
    private Button[] buttons;
    private Vector2[] originalPositions;
    private bool isInitialized = false;

    void Start()
    {
        InitializeEffect();
    }

    void InitializeEffect()
    {
        // 오디오 소스 설정
        if (audioSource == null)
        {
            audioSource = GetComponent<AudioSource>();
            if (audioSource == null)
                audioSource = gameObject.AddComponent<AudioSource>();
        }

        audioSource.playOnAwake = false;

        // 모든 버튼 찾기
        buttons = GetComponentsInChildren<Button>();
        originalPositions = new Vector2[buttons.Length];

        // 각 버튼 초기화
        for (int i = 0; i < buttons.Length; i++)
        {
            SetupButton(buttons[i], i);
        }

        // 순차 등장 애니메이션 시작
        StartCoroutine(PlaySequentialAppearAnimation());

        isInitialized = true;
    }

    void SetupButton(Button button, int index)
    {
        RectTransform rectTransform = button.GetComponent<RectTransform>();

        // 원래 위치 저장
        originalPositions[index] = rectTransform.anchoredPosition;

        // 초기 설정 (화면 밖에서 시작)
        rectTransform.anchoredPosition = new Vector2(-300, originalPositions[index].y);
        button.gameObject.SetActive(false);

        // 기본 색상 설정
        Image buttonImage = button.GetComponent<Image>();
        if (buttonImage != null)
        {
            buttonImage.color = normalColor;
        }

        // 이벤트 트리거 추가
        AddEventTrigger(button, index);
    }

    void AddEventTrigger(Button button, int buttonIndex)
    {
        EventTrigger trigger = button.gameObject.GetComponent<EventTrigger>();
        if (trigger == null)
            trigger = button.gameObject.AddComponent<EventTrigger>();

        // 기존 이벤트 제거
        trigger.triggers.Clear();

        // 호버 시작 이벤트
        EventTrigger.Entry hoverEntry = new EventTrigger.Entry();
        hoverEntry.eventID = EventTriggerType.PointerEnter;
        hoverEntry.callback.AddListener((data) => { OnButtonHover(button); });
        trigger.triggers.Add(hoverEntry);

        // 호버 종료 이벤트
        EventTrigger.Entry exitEntry = new EventTrigger.Entry();
        exitEntry.eventID = EventTriggerType.PointerExit;
        exitEntry.callback.AddListener((data) => { OnButtonExit(button, buttonIndex); });
        trigger.triggers.Add(exitEntry);

        // 클릭 이벤트
        EventTrigger.Entry clickEntry = new EventTrigger.Entry();
        clickEntry.eventID = EventTriggerType.PointerDown;
        clickEntry.callback.AddListener((data) => {
            OnButtonClick(button, data as PointerEventData);
        });
        trigger.triggers.Add(clickEntry);
    }

    IEnumerator PlaySequentialAppearAnimation()
    {
        yield return new WaitForSeconds(0.5f); // 초기 대기

        for (int i = 0; i < buttons.Length; i++)
        {
            Button button = buttons[i];
            button.gameObject.SetActive(true);

            RectTransform rectTransform = button.GetComponent<RectTransform>();

            // 슬라이드 인 애니메이션 (OutBack 이징으로 통통 튀는 효과)
            rectTransform.DOAnchorPosX(originalPositions[i].x, buttonAppearDuration)
                .SetEase(Ease.OutBack);

            // 페이드 인 애니메이션
            CanvasGroup canvasGroup = button.GetComponent<CanvasGroup>();
            if (canvasGroup == null)
                canvasGroup = button.gameObject.AddComponent<CanvasGroup>();

            canvasGroup.alpha = 0;
            canvasGroup.DOFade(1f, buttonAppearDuration);

            yield return new WaitForSeconds(sequentialDelay);
        }
    }

    void OnButtonHover(Button button)
    {
        if (!isInitialized) return;

        PlaySound(hoverSound);

        Image buttonImage = button.GetComponent<Image>();
        RectTransform rectTransform = button.GetComponent<RectTransform>();
        TextMeshProUGUI buttonText = button.GetComponentInChildren<TextMeshProUGUI>();

        // 색상 변경
        if (buttonImage != null)
            buttonImage.DOColor(hoverColor, hoverAnimationDuration);

        // 크기 확대
        rectTransform.DOScale(hoverScale, hoverAnimationDuration);

        // 오른쪽으로 이동
        Vector2 currentPos = rectTransform.anchoredPosition;
        rectTransform.DOAnchorPosX(currentPos.x + hoverMoveX, hoverAnimationDuration);

        // 텍스트 색상 변경 (글로우 효과)
        if (buttonText != null)
            buttonText.DOColor(glowColor, hoverAnimationDuration);

        // 글로우 효과 추가
        AddGlowEffect(button);
    }

    void OnButtonExit(Button button, int buttonIndex)
    {
        if (!isInitialized) return;

        Image buttonImage = button.GetComponent<Image>();
        RectTransform rectTransform = button.GetComponent<RectTransform>();
        TextMeshProUGUI buttonText = button.GetComponentInChildren<TextMeshProUGUI>();

        // 원래 색상으로 복귀
        if (buttonImage != null)
            buttonImage.DOColor(normalColor, hoverAnimationDuration);

        // 원래 크기로 복귀
        rectTransform.DOScale(1f, hoverAnimationDuration);

        // 원래 위치로 복귀
        rectTransform.DOAnchorPosX(originalPositions[buttonIndex].x, hoverAnimationDuration);

        // 텍스트 색상 복귀
        if (buttonText != null)
            buttonText.DOColor(Color.white, hoverAnimationDuration);

        // 글로우 효과 제거
        RemoveGlowEffect(button);
    }

    void OnButtonClick(Button button, PointerEventData eventData)
    {
        if (!isInitialized) return;

        PlaySound(clickSound);

        // 클릭 펀치 애니메이션
        RectTransform rectTransform = button.GetComponent<RectTransform>();
        rectTransform.DOScale(1.02f, 0.1f).OnComplete(() => {
            rectTransform.DOScale(hoverScale, 0.1f); // 호버 크기로 복귀
        });

        // 리플 효과 생성
        StartCoroutine(CreateRippleEffect(button, eventData.position));
    }

    void AddGlowEffect(Button button)
    {
        Outline outline = button.GetComponent<Outline>();
        if (outline == null)
            outline = button.gameObject.AddComponent<Outline>();

        outline.effectColor = new Color(glowColor.r, glowColor.g, glowColor.b, 0f);
        outline.effectDistance = new Vector2(3, 3);
        outline.DOFade(0.8f, hoverAnimationDuration);
    }

    void RemoveGlowEffect(Button button)
    {
        Outline outline = button.GetComponent<Outline>();
        if (outline != null)
        {
            outline.DOFade(0f, hoverAnimationDuration).OnComplete(() => {
                if (outline != null)
                    DestroyImmediate(outline);
            });
        }
    }

    IEnumerator CreateRippleEffect(Button button, Vector2 screenPosition)
    {
        // 리플 오브젝트 생성
        GameObject rippleObj = new GameObject("Ripple");
        rippleObj.transform.SetParent(button.transform, false);

        Image rippleImage = rippleObj.AddComponent<Image>();
        rippleImage.color = new Color(glowColor.r, glowColor.g, glowColor.b, 0.6f);
        rippleImage.raycastTarget = false;
        rippleImage.sprite = CreateCircleSprite();

        RectTransform rippleRect = rippleObj.GetComponent<RectTransform>();

        // 클릭 위치를 로컬 좌표로 변환
        RectTransform buttonRect = button.GetComponent<RectTransform>();
        Vector2 localClickPos;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            buttonRect, screenPosition, null, out localClickPos);

        rippleRect.anchoredPosition = localClickPos;
        rippleRect.sizeDelta = Vector2.zero;

        // 리플 애니메이션
        float maxSize = Mathf.Max(buttonRect.rect.width, buttonRect.rect.height) * 2f;

        rippleRect.DOSizeDelta(new Vector2(maxSize, maxSize), 0.6f);
        rippleImage.DOFade(0f, 0.6f);

        yield return new WaitForSeconds(0.6f);

        if (rippleObj != null)
            DestroyImmediate(rippleObj);
    }

    Sprite CreateCircleSprite()
    {
        // 간단한 원형 스프라이트 생성 (리플 효과용)
        int size = 100;
        Texture2D texture = new Texture2D(size, size);
        Color[] pixels = new Color[size * size];

        Vector2 center = new Vector2(size * 0.5f, size * 0.5f);
        float radius = size * 0.5f;

        for (int x = 0; x < size; x++)
        {
            for (int y = 0; y < size; y++)
            {
                float distance = Vector2.Distance(new Vector2(x, y), center);

                if (distance <= radius)
                {
                    float alpha = Mathf.Clamp01(1f - (distance / radius));
                    alpha = Mathf.SmoothStep(0f, 1f, alpha);
                    pixels[y * size + x] = new Color(1f, 1f, 1f, alpha);
                }
                else
                {
                    pixels[y * size + x] = Color.clear;
                }
            }
        }

        texture.SetPixels(pixels);
        texture.Apply();

        return Sprite.Create(texture, new Rect(0, 0, size, size), new Vector2(0.5f, 0.5f));
    }

    void PlaySound(AudioClip clip)
    {
        if (clip != null && audioSource != null)
            SoundManager.Instance.PlaySFX(clip);
    }

    // ===== 공개 메서드들 =====

    /// <summary>
    /// 애니메이션을 처음부터 다시 재생
    /// </summary>
    public void RestartAnimation()
    {
        if (isInitialized)
        {
            StopAllCoroutines();

            // 모든 버튼 초기 상태로 리셋
            for (int i = 0; i < buttons.Length; i++)
            {
                SetupButton(buttons[i], i);
            }

            StartCoroutine(PlaySequentialAppearAnimation());
        }
    }

    /// <summary>
    /// 모든 애니메이션 효과 일시 중지/재개
    /// </summary>
    public void SetAnimationEnabled(bool enabled)
    {
        isInitialized = enabled;
    }

    /// <summary>
    /// 특정 버튼만 즉시 표시 (애니메이션 없이)
    /// </summary>
    public void ShowButtonImmediately(int buttonIndex)
    {
        if (buttonIndex >= 0 && buttonIndex < buttons.Length)
        {
            Button button = buttons[buttonIndex];
            RectTransform rectTransform = button.GetComponent<RectTransform>();

            button.gameObject.SetActive(true);
            rectTransform.anchoredPosition = originalPositions[buttonIndex];

            CanvasGroup canvasGroup = button.GetComponent<CanvasGroup>();
            if (canvasGroup != null)
                canvasGroup.alpha = 1f;
        }
    }

    /// <summary>
    /// 모든 버튼을 즉시 표시 (애니메이션 없이)
    /// </summary>
    public void ShowAllButtonsImmediately()
    {
        for (int i = 0; i < buttons.Length; i++)
        {
            ShowButtonImmediately(i);
        }
    }
}
