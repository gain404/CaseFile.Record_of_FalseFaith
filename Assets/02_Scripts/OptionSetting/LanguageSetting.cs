using TMPro;
using UnityEngine;
using UnityEngine.Localization.Settings;
using UnityEngine.UI;

public class LanguageSetting : MonoBehaviour
{
    /// <summary>
    /// 옵션창에 있는 언어 선택 세팅을 위한 스크립트
    /// </summary>
    public TMP_Dropdown languageDropdown;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        languageDropdown.onValueChanged.AddListener(ChangeLanguage);//인스펙터 창에서 직접 넣어주는 게 아니라 스크립트로 넣어줌, 추후 언어 추가를 고려해볼 경우
    }

    public void ChangeLanguage(int index)
    {
        LocalizationSettings.SelectedLocale = LocalizationSettings.AvailableLocales.Locales[index];
    }
}
