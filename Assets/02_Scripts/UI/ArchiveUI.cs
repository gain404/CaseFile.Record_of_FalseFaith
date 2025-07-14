using UnityEngine;

/// <summary>
/// 도감 패널 UI 클래스임.
/// 현재는 데이터 연동 없이 빈 화면만 제공.
/// </summary>

public class ArchiveUI : BaseUI
{
    /// <summary>
    ///도감 패널 초기화 시 호출됨.
    /// </summary>

    public override void InitializePanel()
    {
        base.InitializePanel();
        //TODO: 도감 데이터 연동 필요 시 구현.
    }

    /// <summary>
    /// 의존성 주입시 호출됨.
    /// </summary>

    public override void SetupPanelDependencies(UISystemManager manager)
    {
        // TODO: 도감 데이터 매니저가 생기면 연결.
    }
}
