using UnityEngine;
using DG.Tweening;

public class UIAnimator : MonoBehaviour
{
    /// <summary>
    /// UI가 나타나기전에 애니메이션 효과를 주기 위한 스크립트입니다.
    /// </summary>
    public GameObject panel;
    public float animationTime = 0.2f;

    private Tween currentTween;


    public void OpenPanel()
    {
        //우선 판넬을 켜되 스케일을 0으로 해서 안 보이게
        panel.SetActive(true);
        panel.transform.localScale = Vector3.zero;
        
        //기존 애니메이션 있으면 없앰 - 이상해지지 않도록
        currentTween?.Kill();
        //애니메이션을 바꾸고 싶다면 이 부분부터 교체하시면 됩니다.
        currentTween = panel.transform.DOScale(Vector3.one, animationTime).SetEase(Ease.OutBack);
    }

    public void ClosePanel()
    {
        currentTween?.Kill();
        currentTween = panel.transform.DOScale(Vector3.zero, animationTime).SetEase(Ease.InBack).OnComplete(()=>
            {
                panel.SetActive(false);
            });
    }

}
