using UnityEngine;

/// <summary>
/// 각각의 UI 패널이 어떤 종류인지 식별하기 위한 컴포넌트
/// Unity Inspector에서 설정하며, UI 매니저가 패널을 찾을 때 사용
/// </summary>
public class UIPanelIdentifier : MonoBehaviour
{
    [Header("패널 종류 설정")]
    [Tooltip("이 패널이 어떤 종류인지 선택하세요")]
    public UIPanelType panelType;

    /// <summary>
    /// 현재 패널의 타입을 반환하는 메서드
    /// 외부에서 이 패널이 무엇인지 확인할 때 사용
    /// </summary>
    /// <returns>설정된 패널 타입</returns>
    public UIPanelType GetPanelType()
    {
        return panelType;
    }

    /// <summary>
    /// 패널 타입을 동적으로 변경하는 메서드
    /// 런타임에서 패널의 용도가 바뀔 때 사용 (드물게 사용됨)
    /// </summary>
    /// <param name="newType">새로 설정할 패널 타입</param>
    public void ChangePanelType(UIPanelType newType)
    {
        // 기존 타입을 저장해서 로그에 표시
        UIPanelType oldType = panelType;
        // 새로운 타입으로 변경
        panelType = newType;
        // 변경 사항을 로그로 확인
        Debug.Log($"[UIPanelIdentifier] 패널 타입 변경: {oldType} → {newType}");
    }
}