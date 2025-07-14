using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class FadeManager : MonoBehaviour
{
    public static FadeManager Instance { get; private set; }

    [SerializeField] private Image fadeImage;
    [SerializeField] private float fadeDuration = 1.2f;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
        
        SetClearImmediately();
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

    public void SetBlackImmediately() => SetAlpha(1f);
    public void SetClearImmediately() => SetAlpha(0f);

    private void SetAlpha(float alpha)
    {
        var color = fadeImage.color;
        color.a = alpha;
        fadeImage.color = color;
    }
}