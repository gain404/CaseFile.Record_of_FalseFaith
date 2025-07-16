using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class FadeManager : Singleton<FadeManager>
{
    public static FadeManager Instance { get; private set; }

    [SerializeField] private Image fadeImage;
    [SerializeField] private GameObject canvas;
    [SerializeField] private float fadeDuration = 1.2f;

    private void Start()
    {
        SetClearImmediately();
        OffCanvas();
    }

    public void FadeIn(System.Action onComplete = null)
    {
        fadeImage.DOFade(0f, fadeDuration)
            .SetEase(Ease.Linear)
            .OnComplete(() => onComplete?.Invoke());
    }

    public void FadeOut(System.Action onComplete = null)
    {
        fadeImage.DOFade(1f, fadeDuration)
            .SetEase(Ease.Linear)
            .OnComplete(() => onComplete?.Invoke());
    }

    public void OnCanvas()
    {
        canvas.SetActive(true);
    }

    public void OffCanvas()
    {
        canvas.SetActive(false);
    }

    public void SetBlackImmediately() => SetAlpha(1f);
    public void SetClearImmediately() => SetAlpha(0f);

    private void SetAlpha(float alpha)
    {
        var color = fadeImage.color;
        color.a = alpha;
        fadeImage.color = color;
    }
}