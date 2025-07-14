using UnityEngine;


/// <summary>
/// 저장소 패널 UI 클리스임.
/// 현재는 구현된 저장/ 불러오기 기능이 없으므로 빈 창만 표시함.
/// </summary>
public class SaveLoadUI : BaseUI
{
    /// <summary>
    /// 저장소 패널 초기화 시 호출됨.
    /// </summary>
    public override void InitializePanel()
    {
        base.InitializePanel();
        //TODO: 저장소 시스템 구현 시 연결 예정
    }

    /// <summary>
    /// 의존성 주입 시 호출.
    /// </summary>

    public override void SetupPanelDependencies(UISystemManager manager)
    {
        //TODO: 필요 시 종속 매니저 연결
    }
}

