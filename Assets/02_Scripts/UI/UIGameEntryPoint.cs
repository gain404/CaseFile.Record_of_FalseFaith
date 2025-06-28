using UnityEngine;

/// <summary>
/// 게임이 시작될 때 UI 시스템을 초기화하고 설정하는 진입점 클래스
/// 모든 패널들의 의존성을 주입하고 첫 화면을 표시하는 역할
/// </summary>
public class UIGameEntryPoint : MonoBehaviour
{
    [Header("UI 시스템 설정")]
    [Tooltip("전체 UI를 관리하는 매니저")]
    [SerializeField] private UISystemManager uiSystemManager;

    /// <summary>
    /// 게임 시작 시 자동으로 실행되는 메서드
    /// UI 시스템 초기화와 첫 화면 표시를 담당
    /// </summary>
    private void Start()
    {
        // 로그로 게임 시작을 알림
        Debug.Log("[UIGameEntryPoint] 게임 UI 시스템을 시작합니다");

        // UI 매니저가 설정되어 있는지 확인
        if (uiSystemManager == null)
        {
            Debug.LogError("[UIGameEntryPoint] UISystemManager가 설정되지 않았습니다!");
            return; // 초기화 중단
        }

        // 전체 UI 시스템 초기화 실행
        uiSystemManager.InitializeUISystem();

        // 모든 패널에 의존성 주입 실행
        SetupAllPanelDependencies();

        // 게임 시작 시 메인 메뉴를 첫 화면으로 표시
        uiSystemManager.ShowUIPanel(UIPanelType.MainMenu);

        // 초기화 완료 메시지
        Debug.Log("[UIGameEntryPoint] UI 시스템 시작 완료 - 메인 메뉴가 표시됩니다");
    }

    /// <summary>
    /// 모든 패널에 UI 매니저 의존성을 주입하는 메서드
    /// 각 패널이 다른 패널로 이동하거나 시스템 기능을 사용할 수 있게 해줌
    /// </summary>
    private void SetupAllPanelDependencies()
    {
        // 로그로 의존성 주입 시작을 알림
        Debug.Log("[UIGameEntryPoint] 모든 패널에 의존성을 주입합니다");

        // 모든 패널 타입을 하나씩 확인
        foreach (UIPanelType panelType in System.Enum.GetValues(typeof(UIPanelType)))
        {
            // 해당 타입의 패널을 UI 매니저에서 가져오기
            BaseUI targetPanel = uiSystemManager.GetUIPanel(panelType);

            // 패널이 존재하면 의존성 주입 실행
            if (targetPanel != null)
            {
                targetPanel.SetupPanelDependencies(uiSystemManager);
                Debug.Log($"[UIGameEntryPoint] '{panelType}' 패널 의존성 주입 완료");
            }
            else
            {
                // 패널을 찾을 수 없으면 경고 메시지
                Debug.LogWarning($"[UIGameEntryPoint] '{panelType}' 패널을 찾을 수 없어 의존성 주입을 건너뜁니다");
            }
        }

        // 의존성 주입 완료 메시지
        Debug.Log("[UIGameEntryPoint] 모든 패널 의존성 주입 완료");
    }
}