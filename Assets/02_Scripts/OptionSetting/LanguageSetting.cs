using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Localization.Settings;
/// <summary>
/// 옵션창에 있는 언어 선택 세팅을 위한 스크립트(한국어, 영어)
/// </summary>
public class LanguageSetting : MonoBehaviour
{
    [Header("UI References")]
    public TMP_Dropdown languageDropdown;

    private bool initializing = true;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // 드롭다운 이벤트 연결
        languageDropdown.onValueChanged.AddListener(ChangeLanguage);

        // 드롭다운 옵션 초기화
        InitLanguageOptions();
    }

    /// <summary>
    /// 드롭다운을 현재 로컬 설정에 맞게 초기화
    /// </summary>
    private void InitLanguageOptions()
    {
        // 드롭다운 옵션 초기화
        languageDropdown.ClearOptions();

        // 로컬 언어 리스트에서 이름을 가져옴 (예: English, 한국어)
        var options = new List<string>();
        int currentLocaleIndex = 0;

        for (int i = 0; i < LocalizationSettings.AvailableLocales.Locales.Count; i++)
        {
            var locale = LocalizationSettings.AvailableLocales.Locales[i];
            options.Add(locale.Identifier.CultureInfo.NativeName);

            if (LocalizationSettings.SelectedLocale == locale)
            {
                currentLocaleIndex = i;
            }
        }
        languageDropdown.AddOptions(options);
        languageDropdown.value = currentLocaleIndex;
        languageDropdown.RefreshShownValue();

        initializing = false;
    }

    /// <summary>
    /// 드롭다운 선택 변경 시 언어 변경
    /// </summary>
    public void ChangeLanguage(int index)
    {
        // Start 시 초기화 중 호출되는 값은 무시
        if (initializing) return;

        // 언어 변경
        LocalizationSettings.SelectedLocale = LocalizationSettings.AvailableLocales.Locales[index];
    }
}
