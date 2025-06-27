using UnityEngine;

/// <summary>
/// 모든 UI 패널들이 상속받아야 하는 기본 베이스 클래스
/// 공통적으로 필요한 Show/Hide 기능과 초기화 기능을 제공함
/// </summary>
public abstract class BaseUI : MonoBehaviour
{
    /// <summary>
    /// UI 패널을 화면에 표시하는 메서드
    /// 각 패널마다 다른 방식으로 표시될 수 있음 (페이드인, 슬라이드 등)
    /// </summary>
    public virtual void ShowPanel()
    {
        // GameObject를 활성화해서 화면에 보이게 만들기
        gameObject.SetActive(true);
        // 로그로 어떤 패널이 열렸는지 확인
        Debug.Log($"[BaseUI] {gameObject.name} 패널이 열렸습니다");
    }

    /// <summary>
    /// UI 패널을 화면에서 숨기는 메서드
    /// 메모리에서 삭제하지 않고 단순히 보이지 않게만 처리
    /// </summary>
    public virtual void HidePanel()
    {
        // GameObject를 비활성화해서 화면에서 숨기기
        gameObject.SetActive(false);
        // 로그로 어떤 패널이 닫혔는지 확인
        Debug.Log($"[BaseUI] {gameObject.name} 패널이 닫혔습니다");
    }

    /// <summary>
    /// UI 패널이 처음 생성될 때 한 번만 실행되는 초기화 메서드
    /// 버튼 이벤트 연결, 초기 데이터 설정 등을 여기서 처리
    /// </summary>
    public virtual void InitializePanel()
    {
        // 로그로 초기화 완료 확인
        Debug.Log($"[BaseUI] {gameObject.name} 패널 초기화 완료");
    }

    /// <summary>
    /// UI 매니저와의 연결을 설정하는 메서드
    /// 각 패널이 다른 패널로 이동하거나 기능을 사용할 때 필요
    /// </summary>
    /// <param name="manager">현재 게임의 UI 매니저 참조</param>
    public virtual void SetupPanelDependencies(UISystemManager manager)
    {
        // 기본적으로는 아무것도 하지 않음
        // 필요한 패널에서 오버라이드해서 사용
        Debug.Log($"[BaseUI] {gameObject.name} 패널 의존성 설정 완료");
    }
}