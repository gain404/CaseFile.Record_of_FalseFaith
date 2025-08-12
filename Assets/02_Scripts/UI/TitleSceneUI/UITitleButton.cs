using DG.Tweening;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UITitleButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler
{

    [Header("UI References")]
    public Image buttonImage;           // 메인 버튼 이미지
    public TextMeshProUGUI buttonText;  // 버튼 텍스트
    public Image accentLine;            // 밑줄

    [Header("Colors")]
    public Color normalColor = new Color(0.16f, 0.16f, 0.24f, 1f);      // 기본 색상
    public Color hoverColor = new Color(0.23f, 0.23f, 0.31f, 1f);       // 호버 색상
    public Color glowColor = new Color(0f, 1f, 0.53f, 1f);              // 글로우 색상
    public Color textNormalColor = Color.white;                          // 텍스트 기본 색상

    [Header("Animation Settings")]
    public float hoverScale = 1.05f;           // 호버 시 크기
    public float hoverMoveX = 10f;             // 호버 시 이동 거리
    public float animationDuration = 0.3f;     // 애니메이션 지속시간
    public float accentLineMaxWidth = 0.8f;    // 밑줄 최대 너비

    [Header("Audio")]
    public AudioClip hoverSound;
    public AudioClip clickSound;

    // Private 변수들
    private Button button;
    private RectTransform rectTransform;
    private Vector2 originalPosition;
    private Vector3 originalScale;
    private bool isHovered = false;
    private AudioSource audioSource;
    private Outline glowOutline;

    void Start()
    {
        SetupButton();
    }

    void SetupButton()
    {
        // 컴포넌트 참조
        button = GetComponent<Button>();
        rectTransform = GetComponent<RectTransform>();
        audioSource = GetComponent<AudioSource>();

        if (audioSource == null)
            audioSource = gameObject.AddComponent<AudioSource>();

        // 자동으로 컴포넌트 찾기 (할당되지 않은 경우)
        if (buttonImage == null)
            buttonImage = GetComponent<Image>();

        if (buttonText == null)
            buttonText = GetComponentInChildren<TextMeshProUGUI>();

        if (accentLine == null)
            accentLine = transform.Find("AccentLine")?.GetComponent<Image>();

        // 초기값 저장
        originalPosition = rectTransform.anchoredPosition;
        originalScale = rectTransform.localScale;

        // 초기 색상 설정
        if (buttonImage != null)
            buttonImage.color = normalColor;

        if (buttonText != null)
            buttonText.color = textNormalColor;

        // 액센트 라인 초기 설정
        if (accentLine != null)
        {
            accentLine.type = Image.Type.Filled;
            accentLine.fillMethod = Image.FillMethod.Horizontal;
            accentLine.fillAmount = 0f;
            accentLine.color = glowColor;
        }

        // 버튼 클릭 이벤트는 Button 컴포넌트에서 설정
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (!button.interactable) return;

        isHovered = true;
        PlaySound(hoverSound);

        // 색상 애니메이션
        if (buttonImage != null)
            buttonImage.DOColor(hoverColor, animationDuration);

        // 크기 및 위치 애니메이션
        rectTransform.DOScale(originalScale * hoverScale, animationDuration);
        rectTransform.DOAnchorPosX(originalPosition.x + hoverMoveX, animationDuration);

        // 텍스트 색상 변경
        if (buttonText != null)
            buttonText.DOColor(glowColor, animationDuration);

        // 액센트 라인 애니메이션
        if (accentLine != null)
            accentLine.DOFillAmount(accentLineMaxWidth, animationDuration);

        // 글로우 효과 추가
        AddGlowEffect();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (!button.interactable) return;

        isHovered = false;

        // 원래 색상으로 복귀
        if (buttonImage != null)
            buttonImage.DOColor(normalColor, animationDuration);

        // 크기 및 위치 복귀
        rectTransform.DOScale(originalScale, animationDuration);
        rectTransform.DOAnchorPosX(originalPosition.x, animationDuration);

        // 텍스트 색상 복귀
        if (buttonText != null)
            buttonText.DOColor(textNormalColor, animationDuration);

        // 액센트 라인 숨김
        if (accentLine != null)
            accentLine.DOFillAmount(0f, animationDuration);

        // 글로우 효과 제거
        RemoveGlowEffect();
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (!button.interactable) return;

        PlaySound(clickSound);

        // 클릭 애니메이션 (펀치 효과)
        float targetScale = isHovered ? hoverScale * 1.02f : 1.02f;
        float finalScale = isHovered ? hoverScale : 1f;

        rectTransform.DOScale(originalScale * targetScale, 0.1f).OnComplete(() => {
            rectTransform.DOScale(originalScale * finalScale, 0.1f);
        });

        // 리플 효과
        StartCoroutine(CreateRippleEffect(eventData.position));
    }

    void AddGlowEffect()
    {
        if (glowOutline == null)
            glowOutline = gameObject.AddComponent<Outline>();

        glowOutline.effectColor = new Color(glowColor.r, glowColor.g, glowColor.b, 0f);
        glowOutline.effectDistance = new Vector2(3, 3);
        glowOutline.DOFade(0.8f, animationDuration);
    }

    void RemoveGlowEffect()
    {
        if (glowOutline != null)
        {
            glowOutline.DOFade(0f, animationDuration).OnComplete(() => {
                if (!isHovered && glowOutline != null)
                {
                    DestroyImmediate(glowOutline);
                    glowOutline = null;
                }
            });
        }
    }

    IEnumerator CreateRippleEffect(Vector2 screenPosition)
    {
        // 리플 오브젝트 생성
        GameObject rippleObj = new GameObject("Ripple");
        rippleObj.transform.SetParent(transform, false);

        // 리플 이미지 컴포넌트
        Image rippleImage = rippleObj.AddComponent<Image>();
        rippleImage.color = new Color(glowColor.r, glowColor.g, glowColor.b, 0.6f);
        rippleImage.raycastTarget = false;
        rippleImage.sprite = CreateCircleSprite();

        RectTransform rippleRect = rippleObj.GetComponent<RectTransform>();

        // 클릭 위치를 로컬 좌표로 변환
        Vector2 localClickPos;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            rectTransform, screenPosition, null, out localClickPos);

        rippleRect.anchoredPosition = localClickPos;
        rippleRect.sizeDelta = Vector2.zero;

        // 리플 애니메이션
        float maxSize = Mathf.Max(rectTransform.rect.width, rectTransform.rect.height) * 2f;

        // 크기 애니메이션
        rippleRect.DOSizeDelta(new Vector2(maxSize, maxSize), 0.6f);

        // 페이드 아웃 애니메이션
        rippleImage.DOFade(0f, 0.6f);

        yield return new WaitForSeconds(0.6f);

        // 리플 오브젝트 삭제
        if (rippleObj != null)
            DestroyImmediate(rippleObj);
    }

    Sprite CreateCircleSprite()
    {
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
                    // 부드러운 가장자리
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
            audioSource.PlayOneShot(clip);
    }

    // 퍼블릭 메서드들 (외부에서 호출 가능)
    public void SetButtonText(string text)
    {
        if (buttonText != null)
            buttonText.text = text;
    }

    public void SetButtonColors(Color normal, Color hover, Color glow)
    {
        normalColor = normal;
        hoverColor = hover;
        glowColor = glow;

        if (buttonImage != null && !isHovered)
            buttonImage.color = normalColor;
    }

    public void SetInteractable(bool interactable)
    {
        if (button != null)
            button.interactable = interactable;
    }
}
