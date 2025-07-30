using UnityEngine;

public class UISetting : MonoBehaviour
{
    [Header("Component References")]
    [SerializeField] private GameObject settingsPanel;

    [SerializeField] private SoundSetting soundSetting;
    [SerializeField] private DisplaySetting displaySetting;
    [SerializeField] private LanguageSetting languageSetting;

    private bool isOpen = false;

    private void Start()
    {
        settingsPanel.SetActive(false);
        LoadAllSettings();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))//임시로 esc키 할당
        {
            ToggleSettingsPanel();
        }
    }

    public void ToggleSettingsPanel()
    {
        isOpen = !isOpen;
        settingsPanel.SetActive(isOpen);
        Time.timeScale = isOpen ? 0f : 1f; // 일시정지
    }

    /// <summary>
    /// 모든 설정을 불러옴 (PlayerPrefs 등)
    /// </summary>
    public void LoadAllSettings()
    {
        // 각 서브 설정 스크립트에 Load() 함수 만들고 호출
        soundSetting?.LoadSettings();
        // displaySetting은 Start()에서 초기화되므로 생략
        // languageSetting도 Start()에서 이미 Dropdown 설정하므로 생략
    }

    /// <summary>
    /// 모든 설정을 저장
    /// </summary>
    public void SaveAllSettings()
    {
        soundSetting?.SaveSettings();
        // 추후 디스플레이/언어도 SaveSettings() 추가 가능
    }
}
