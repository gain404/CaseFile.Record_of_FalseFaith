using UnityEngine;
/// <summary>
/// 게임 내에서 사용할 수 있는 UI 패널들의 종류를 정의하는 열거형
/// 새로운 패널이 추가될 때마다 여기에 타입을 추가해야 함
/// </summary>
public enum UIPanelType
{
    MainMenu,    // 메인 메뉴 화면 - 게임 시작 시 첫 화면
    Option,      // 옵션 설정 화면 - 사운드, 그래픽 등 설정
    SaveLoad,    // 저장/로드 화면 - 게임 진행상황 저장/불러오기
    Archive,     // 도감 화면 - 수집한 아이템이나 정보 확인
    Game         // 실제 게임 플레이 화면
}
