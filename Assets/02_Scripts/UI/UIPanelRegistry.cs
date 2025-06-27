using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// 게임 내의 모든 UI 패널들을 등록하고 관리하는 레지스트리 클래스
/// 패널들을 타입별로 분류해서 빠르게 찾을 수 있도록 도와줌
/// </summary>
public class UIPanelRegistry : MonoBehaviour
{
    [Header("등록할 패널들")]
    [Tooltip("게임에서 사용할 모든 UI 패널들을 여기에 드래그해서 등록하세요")]
    [SerializeField] private BaseUI[] registeredPanels;

    // 패널 타입을 키로 하고 실제 패널 객체를 값으로 하는 딕셔너리
    private Dictionary<UIPanelType, BaseUI> panelStorage = new Dictionary<UIPanelType, BaseUI>();

    /// <summary>
    /// 등록된 모든 패널들을 초기화하고 딕셔너리에 저장하는 메서드
    /// 게임 시작 시 한 번만 실행되며, 모든 패널을 사용 준비 상태로 만듦
    /// </summary>
    public void InitializeAllPanels()
    {
        // 로그로 초기화 시작을 알림
        Debug.Log("[UIPanelRegistry] 모든 패널 초기화를 시작합니다");

        // 등록된 패널 배열을 하나씩 확인
        foreach (var panel in registeredPanels)
        {
            // 패널이 null인지 확인 (Inspector에서 빈 슬롯이 있을 수 있음)
            if (panel == null)
            {
                Debug.LogWarning("[UIPanelRegistry] null 패널이 발견되어 건너뜁니다");
                continue; // 다음 패널로 넘어가기
            }

            // 패널에서 식별자 컴포넌트 찾기
            var identifier = panel.GetComponent<UIPanelIdentifier>();
            if (identifier == null)
            {
                // 식별자가 없으면 경고 메시지 출력
                Debug.LogWarning($"[UIPanelRegistry] '{panel.name}' 패널에 UIPanelIdentifier가 없습니다");
                continue; // 다음 패널로 넘어가기
            }

            // 패널 초기화 실행
            panel.InitializePanel();
            // 처음에는 모든 패널을 숨김 상태로 설정
            panel.HidePanel();
            // 딕셔너리에 패널 타입과 패널 객체 저장
            panelStorage[identifier.GetPanelType()] = panel;

            // 성공적으로 등록된 패널 정보를 로그로 출력
            Debug.Log($"[UIPanelRegistry] '{identifier.GetPanelType()}' 패널 등록 완료");
        }

        // 전체 초기화 완료 메시지
        Debug.Log($"[UIPanelRegistry] 총 {panelStorage.Count}개 패널 초기화 완료");
    }

    /// <summary>
    /// 특정 타입의 패널을 찾아서 반환하는 메서드
    /// UI 매니저에서 패널을 표시하거나 숨길 때 사용
    /// </summary>
    /// <param name="panelType">찾고 싶은 패널의 타입</param>
    /// <returns>해당 타입의 패널 객체 (없으면 null 반환)</returns>
    public BaseUI FindPanelByType(UIPanelType panelType)
    {
        // 딕셔너리에서 해당 타입의 패널을 찾기 시도
        if (panelStorage.TryGetValue(panelType, out BaseUI foundPanel))
        {
            // 찾았으면 해당 패널 반환
            return foundPanel;
        }

        // 못 찾았으면 경고 메시지 출력하고 null 반환
        Debug.LogWarning($"[UIPanelRegistry] '{panelType}' 타입의 패널을 찾을 수 없습니다");
        return null;
    }

    /// <summary>
    /// 현재 등록된 모든 패널의 목록을 반환하는 메서드
    /// 디버깅이나 전체 패널 제어할 때 사용
    /// </summary>
    /// <returns>등록된 모든 패널들의 컬렉션</returns>
    public IEnumerable<BaseUI> GetAllRegisteredPanels()
    {
        // 딕셔너리의 모든 값(패널 객체들)을 반환
        return panelStorage.Values;
    }

    /// <summary>
    /// 특정 패널이 등록되어 있는지 확인하는 메서드
    /// 패널 존재 여부를 미리 확인하고 싶을 때 사용
    /// </summary>
    /// <param name="panelType">확인하고 싶은 패널 타입</param>
    /// <returns>해당 패널이 등록되어 있으면 true, 아니면 false</returns>
    public bool IsPanelRegistered(UIPanelType panelType)
    {
        // 딕셔너리에 해당 키가 있는지 확인
        bool isRegistered = panelStorage.ContainsKey(panelType);
        // 결과를 로그로 출력
        Debug.Log($"[UIPanelRegistry] '{panelType}' 패널 등록 상태: {isRegistered}");
        return isRegistered;
    }
}