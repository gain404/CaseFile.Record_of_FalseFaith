using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// 전체 UI 시스템을 관리하는 최상위 매니저 클래스
/// 패널 전환, 표시/숨김, 상태 관리 등 모든 UI 관련 기능을 총괄함
/// </summary>
public class UISystemManager : MonoBehaviour
{
    [Header("UI 시스템 구성요소")]
    [Tooltip("모든 패널을 관리하는 레지스트리")]
    [SerializeField] private UIPanelRegistry panelRegistry;

    // 현재 화면에 표시되고 있는 패널을 추적
    private BaseUI currentActivePanel;
    // 패널 전환 히스토리를 저장 (뒤로가기 기능용)
    private Stack<UIPanelType> panelNavigationHistory = new Stack<UIPanelType>();

    /// <summary>
    /// UI 시스템 전체를 초기화하는 메서드
    /// 게임 시작 시 가장 먼저 실행되어야 함
    /// </summary>
    public void InitializeUISystem()
    {
        // 로그로 초기화 시작을 알림
        Debug.Log("[UISystemManager] UI 시스템 초기화를 시작합니다");

        // 패널 레지스트리가 설정되어 있는지 확인
        if (panelRegistry == null)
        {
            Debug.LogError("[UISystemManager] UIPanelRegistry가 설정되지 않았습니다!");
            return; // 초기화 중단
        }

        // 레지스트리를 통해 모든 패널 초기화
        panelRegistry.InitializeAllPanels();

        // 현재 활성 패널을 null로 초기화
        currentActivePanel = null;
        // 네비게이션 히스토리 초기화
        panelNavigationHistory.Clear();

        // 초기화 완료 메시지
        Debug.Log("[UISystemManager] UI 시스템 초기화가 완료되었습니다");
    }

    /// <summary>
    /// 지정된 타입의 패널을 화면에 표시하는 메서드
    /// 기존에 열려있던 패널은 자동으로 숨겨짐
    /// </summary>
    /// <param name="panelType">표시할 패널의 타입</param>
    public void ShowUIPanel(UIPanelType panelType)
    {
        // 로그로 패널 전환 시작을 알림
        Debug.Log($"[UISystemManager] '{panelType}' 패널을 표시합니다");

        // 레지스트리에서 해당 패널 찾기
        BaseUI targetPanel = panelRegistry.FindPanelByType(panelType);
        if (targetPanel == null)
        {
            Debug.LogError($"[UISystemManager] '{panelType}' 패널을 찾을 수 없어 표시에 실패했습니다");
            return; // 실패 시 종료
        }

        // 현재 활성 패널이 있으면 먼저 숨기기
        if (currentActivePanel != null)
        {
            currentActivePanel.HidePanel();
            Debug.Log($"[UISystemManager] 이전 패널을 숨겼습니다");
        }

        // 새로운 패널을 현재 활성 패널로 설정
        currentActivePanel = targetPanel;
        // 패널을 화면에 표시
        currentActivePanel.ShowPanel();

        // 네비게이션 히스토리에 추가 (뒤로가기용)
        panelNavigationHistory.Push(panelType);

        // 성공 메시지 출력
        Debug.Log($"[UISystemManager] '{panelType}' 패널 표시 완료");
    }

    /// <summary>
    /// 지정된 타입의 패널을 숨기는 메서드
    /// 해당 패널이 현재 활성 패널이면 활성 패널을 null로 설정
    /// </summary>
    /// <param name="panelType">숨길 패널의 타입</param>
    public void HideUIPanel(UIPanelType panelType)
    {
        // 로그로 패널 숨김 시작을 알림
        Debug.Log($"[UISystemManager] '{panelType}' 패널을 숨깁니다");

        // 레지스트리에서 해당 패널 찾기
        BaseUI targetPanel = panelRegistry.FindPanelByType(panelType);
        if (targetPanel == null)
        {
            Debug.LogError($"[UISystemManager] '{panelType}' 패널을 찾을 수 없어 숨김에 실패했습니다");
            return; // 실패 시 종료
        }

        // 패널 숨기기 실행
        targetPanel.HidePanel();

        // 숨긴 패널이 현재 활성 패널이었다면 활성 패널을 null로 설정
        if (currentActivePanel == targetPanel)
        {
            currentActivePanel = null;
            Debug.Log("[UISystemManager] 현재 활성 패널이 해제되었습니다");
        }

        // 성공 메시지 출력
        Debug.Log($"[UISystemManager] '{panelType}' 패널 숨김 완료");
    }

    /// <summary>
    /// 특정 타입의 패널 객체를 가져오는 메서드
    /// 다른 스크립트에서 패널에 직접 접근해야 할 때 사용
    /// </summary>
    /// <param name="panelType">가져올 패널의 타입</param>
    /// <returns>해당 패널 객체 (없으면 null)</returns>
    public BaseUI GetUIPanel(UIPanelType panelType)
    {
        // 레지스트리에서 패널 찾아서 반환
        BaseUI targetPanel = panelRegistry.FindPanelByType(panelType);

        if (targetPanel == null)
        {
            Debug.LogWarning($"[UISystemManager] '{panelType}' 패널을 가져올 수 없습니다");
        }

        return targetPanel;
    }

    /// <summary>
    /// 현재 화면에 표시되고 있는 활성 패널을 반환하는 메서드
    /// 현재 어떤 화면이 열려있는지 확인할 때 사용
    /// </summary>
    /// <returns>현재 활성 패널 (없으면 null)</returns>
    public BaseUI GetCurrentActivePanel()
    {
        return currentActivePanel;
    }

    /// <summary>
    /// 현재 표시되고 있는 모든 패널을 숨기는 메서드
    /// 게임 시작이나 특별한 상황에서 모든 UI를 제거할 때 사용
    /// </summary>
    public void HideAllUIPanels()
    {
        // 로그로 전체 패널 숨김 시작을 알림
        Debug.Log("[UISystemManager] 모든 패널을 숨깁니다");

        // 레지스트리에서 등록된 모든 패널 가져오기
        var allPanels = panelRegistry.GetAllRegisteredPanels();

        // 각 패널을 하나씩 숨기기
        foreach (var panel in allPanels)
        {
            if (panel != null)
            {
                panel.HidePanel();
            }
        }

        // 현재 활성 패널도 null로 설정
        currentActivePanel = null;
        // 네비게이션 히스토리도 초기화
        panelNavigationHistory.Clear();

        // 완료 메시지 출력
        Debug.Log("[UISystemManager] 모든 패널 숨김 완료");
    }

    /// <summary>
    /// 이전 패널로 돌아가는 메서드 (뒤로가기 기능)
    /// 네비게이션 히스토리를 사용해서 이전 화면으로 이동
    /// </summary>
    public void NavigateBackToPreviousPanel()
    {
        // 히스토리에 2개 이상의 항목이 있는지 확인 (현재+이전)
        if (panelNavigationHistory.Count < 2)
        {
            Debug.LogWarning("[UISystemManager] 돌아갈 이전 패널이 없습니다");
            return;
        }

        // 현재 패널을 히스토리에서 제거
        panelNavigationHistory.Pop();
        // 이전 패널 타입을 가져오기
        UIPanelType previousPanelType = panelNavigationHistory.Pop();

        // 이전 패널로 이동 (ShowUIPanel은 자동으로 히스토리에 추가함)
        ShowUIPanel(previousPanelType);

        Debug.Log($"[UISystemManager] 이전 패널 '{previousPanelType}'로 돌아갔습니다");
    }

    /// <summary>
    /// UI 시스템의 현재 상태를 저장하는 메서드
    /// 게임 저장 시 UI 상태도 함께 저장할 때 사용
    /// </summary>
    public void SaveUISystemState()
    {
        // TODO: 실제 저장 로직 구현 필요
        Debug.Log("[UISystemManager] UI 상태 저장 기능 - 구현 예정");

        // 현재 활성 패널 정보 저장
        if (currentActivePanel != null)
        {
            Debug.Log($"[UISystemManager] 현재 활성 패널: {currentActivePanel.name}");
        }
    }

    /// <summary>
    /// 패널 간 전환 애니메이션을 재생하는 메서드
    /// 부드러운 화면 전환 효과를 제공
    /// </summary>
    /// <param name="fromPanel">전환 시작 패널 타입</param>
    /// <param name="toPanel">전환 목표 패널 타입</param>
    public void PlayPanelTransitionAnimation(UIPanelType fromPanel, UIPanelType toPanel)
    {
        // TODO: 실제 애니메이션 로직 구현 필요
        Debug.Log($"[UISystemManager] 패널 전환 애니메이션: {fromPanel} → {toPanel}");

        // 애니메이션 완료 후 실제 패널 전환 실행
        ShowUIPanel(toPanel);
    }
}