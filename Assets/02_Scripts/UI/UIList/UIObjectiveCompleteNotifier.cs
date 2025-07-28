using System.Collections;
using TMPro;
using UnityEngine;

/// <summary>
/// 목표 달성시 자동으로 화면에 달성 됐다고 알려주기 위한 스크립트입니다.
/// </summary>
public class UIObjectiveCompleteNotifier : MonoBehaviour
{
    [SerializeField] private Transform popupRoot; // 생성 위치
    [SerializeField] private GameObject objectiveItemPrefab; // 기존 ObjectiveUIItem 프리팹
    [SerializeField] private float popupDuration = 2.5f;

    public void ShowCompletedObjective(ObjectiveData data)
    {
        GameObject popup = Instantiate(objectiveItemPrefab, popupRoot);
        ObjectiveUIItem item = popup.GetComponent<ObjectiveUIItem>();

        item.Setup(data);
        item.MarkAsCompleted(false); // 애니메이션 없이 완료 상태로 표시

        // 팝업용으로 진행도 텍스트 등은 숨김 처리
        popup.transform.localScale = Vector3.one;

        StartCoroutine(AutoDestroy(popup, popupDuration));
    }

    private IEnumerator AutoDestroy(GameObject obj, float delay)
    {
        yield return new WaitForSeconds(delay);
        Destroy(obj);
    }
}
