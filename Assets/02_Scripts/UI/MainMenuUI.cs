using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

/// <summary>
/// 메인 메뉴 UI를 제어하는 클래스입니다.
/// 게임 시작, 옵션, 저장소, 도감, 종료 버튼의 클릭 이벤트를 처리합니다.
/// </summary>

public class MainMenuUI : BaseUI
{
    [SerializeField] private Button startGameButton; // 게임 시작 버튼
    [SerializeField] private Button optionButton; // 옵션 메뉴로 이동하는 버튼
    [SerializeField] private Button saveLoadButton; // 저장소 패널로 이동하는 버튼
    [SerializeField] private Button archiveButton; // 도감 패널로 이동하는 버튼
    [SerializeField] private Button quitButton; // 게임 종료 버튼

    private UISystemManager uiSystemManager;

    /// <summary>
    /// UI 패널 초기화 시 호출 됨. 버튼 이벤트 연결함.
    /// </summary>

    public override void InitializePanel()
    {
        base.InitializePanel();

        // 각 버튼에 클릭 이벤트 할당
        startGameButton.onClick.AddListener(OnClickStartGame);
        optionButton.onClick.AddListener(OnClickOption);
        saveLoadButton.onClick.AddListener(OnClickSaveLoad);
        archiveButton.onClick.AddListener(OnClickArchive);
        quitButton.onClick.AddListener(OnClickQuit);
    }

    /// <summary>
    /// UI 시스템 매니저를 주입받음.
    /// </summary>

    public override void SetupPanelDependencies(UISystemManager manager)
    {
        this.uiSystemManager = manager;
    }

    /// <summary>
    /// 게임 시작 버튼 클릭 시 호출됨. 메인 게임 씬으로 전환함.
    /// </summary>

    private void OnClickStartGame()
    {
        SceneManager.LoadScene("MainGame");
    }

    /// <summary>
    /// 옵션 패널 보여줌.
    /// </summary>

    private void OnClickOption()
    {
        uiSystemManager.ShowUIPanel(UIPanelType.Option);
    }


    /// <summary>
    /// 저장소 패널을 보여줌.
    /// </summary>

    private void OnClickSaveLoad()
    {
        uiSystemManager.ShowUIPanel(UIPanelType.SaveLoad);
    }

    /// <summary>
    /// 도감 패널을 보여줌.
    /// </summary>

    private void OnClickArchive()
    {
        uiSystemManager.ShowUIPanel(UIPanelType.Archive);
    }

    /// <summary>
    /// 게임 종료 버튼 클릭 시 호출됨. 게임 종료.
    /// </summary>

    private void OnClickQuit()
    {
        Application.Quit();
        Debug.Log("게임을 종료합니다.");
    }
}
