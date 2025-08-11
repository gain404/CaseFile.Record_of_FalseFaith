using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class UIFadePanel : MonoBehaviour
{
    [SerializeField] private Image fadeImage;

    private void Start()
    {
        SetClearImmediately();
        transform.SetAsFirstSibling();
    }

    private void OnDestroy()
    {
        DOTween.Kill(this);
    }

    public void Fade(float targetAlpha, float fadeDuration, System.Action onComplete = null)
    {
        fadeImage.DOFade(targetAlpha, fadeDuration)
            .SetEase(Ease.Linear)
            .SetLink(gameObject)
            .OnComplete(() => onComplete?.Invoke());
    }
    
    public void SetBlackImmediately() => SetAlpha(1f);
    public void SetClearImmediately() => SetAlpha(0f);

    public void AllFade()
    {
        transform.SetAsLastSibling();
        DOVirtual.DelayedCall(3.0f, () => transform.SetAsFirstSibling())
            .SetLink(gameObject);
    }

    private void SetAlpha(float alpha)
    {
        var color = fadeImage.color;
        color.a = alpha;
        fadeImage.color = color;
    }
}
