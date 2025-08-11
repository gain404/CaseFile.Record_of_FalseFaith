using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class UIFadePanel : MonoBehaviour
{
    [SerializeField] private Image fadeImage;

    private void Start()
    {
        SetClearImmediately();
    }

    public void Fade(float targetAlpha, float fadeDuration, System.Action onComplete = null)
    {
        fadeImage.DOFade(targetAlpha, fadeDuration)
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