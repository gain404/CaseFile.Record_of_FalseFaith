using System.Collections;
using DG.Tweening;
using TMPro;
using UnityEngine;

/// <summary>
/// 목표 달성시 자동으로 화면에 달성 됐다고 알려주기 위한 스크립트입니다.
/// </summary>
public class UIObjectiveCompleteNotifier : MonoBehaviour
{
    [SerializeField] private Transform popupRoot; // 생성 위치
    [SerializeField] private GameObject objectiveItemPrefab; // 기존 ObjectiveUIItem 프리팹

    [SerializeField] private float slideDuration = 0.3f;
    [SerializeField] private float displayDuration = 2f;
    [SerializeField] private Vector2 slideOffset = new Vector2(0, 100); // 위쪽에서 슬라이드 인

    public void ShowCompletedObjective(ObjectiveData data)
    {
        GameObject popup = Instantiate(objectiveItemPrefab, popupRoot);
        RectTransform rect = popup.GetComponent<RectTransform>();
        ObjectiveUIItem item = popup.GetComponent<ObjectiveUIItem>();

        item.Setup(data);
        item.MarkAsCompleted(false);// 애니메이션 없이 완료 상태로 표시

        // 팝업용으로 진행도 텍스트 등은 숨김 처리
        rect.anchoredPosition += slideOffset;

        // 슬라이드 인 → 대기 → 슬라이드 아웃 → 제거
        Sequence seq = DOTween.Sequence();
        seq.Append(rect.DOAnchorPos(rect.anchoredPosition - slideOffset, slideDuration).SetEase(Ease.OutCubic)) // 슬라이드 인
           .AppendInterval(displayDuration) // 대기
           .Append(rect.DOAnchorPos(rect.anchoredPosition, slideDuration).SetEase(Ease.InCubic)) // 슬라이드 아웃
           .OnComplete(() => Destroy(popup)); // 제거
    }

    //DOTween사용으로 이제 쓰지 않음
    private IEnumerator AutoDestroy(GameObject obj, float delay)
    {
        yield return new WaitForSeconds(delay);
        Destroy(obj);
    }
}
