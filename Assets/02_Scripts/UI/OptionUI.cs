using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 옵션 메뉴 UI를 제어하는 클래스임.
/// 마스터 볼륨, 배경 음악, 효과음 볼륨을 조절하는 슬라이더를 제어함.
/// </summary>

public class OptionUI : BaseUI
{
    [SerializeField] private Slider masterVolumeSlider; // 마스터 볼륨 슬라이더
    [SerializeField] private Slider backgroundMusicSlider; // 배경음 슬라이더
    [SerializeField] private Slider soundEffectSlider; // 배경음 슬라이더

    /// <summary>
    /// 옵션 UI 패널 초기화 시 호출됨. 슬라이더 초기값 설정 및 이벤트 연결 예정.
    /// </summary>

    public override void InitializePanel()
    {
        base.InitializePanel();
        //TODO: 오디오 시스템 연결 시 초기값 설정 및 이벤트 연결 필요
    }

    /// <summary>
    /// 의존성 주입을 위한 UI 시스템 매니저 연결
    /// </summary>

    public override void SetupPanelDependencies(UISystemManager manager)
    {
        // TODO: 오디오 시스템 또는 볼륨 매니저가 존재할 경우 연결 필요
    }

    // TODO: 슬라이더 이벤트 핸들러 추가
}
