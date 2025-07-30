using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 화면 해상도 및 전체화면 설정을 관리하는 스크립트
/// - Dropdown으로 해상도 설정
/// - Toggle으로 전체화면 On/Off
/// </summary>
public class DisplaySetting : MonoBehaviour
{
    public TMP_Dropdown resolutionDropdown;// 해상도 선택 드롭다운
    public Toggle fullscreenToggle;// 전체화면 토글
    private Resolution[] resolutions;// 지원되는 해상도 목록

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        resolutions = Screen.resolutions;//스크린 해상도
        // 기존 옵션 제거
        resolutionDropdown.ClearOptions();

        // 드롭다운에 표시될 텍스트 리스트
        List<string> options = new List<string>();

        int current = 0;
        for (int i = 0; i < resolutions.Length; i++)
        {
            var option = resolutions[i].width + " x " + resolutions[i].height;//유저가 옵션에서 볼 텍스트

            // 중복된 해상도 옵션 제거 (같은 해상도가 여러 번 나올 수 있음)
            if (!options.Contains(option))
            {
                options.Add(option);
            }

            if (resolutions[i].width == Screen.currentResolution.width &&
                resolutions[i].height == Screen.currentResolution.height)
            {
                current = i;//지금 해상도와 맞는 순서 찾음 보통은(1920*1080)에 해당하는 인덱스가 들어갈 겁니다.
            }
        }

        // 드롭다운에 옵션 적용
        resolutionDropdown.AddOptions(options);

        // 해상도 옵션 바꿨으면 적용시켜주기
        resolutionDropdown.value = current;
        resolutionDropdown.RefreshShownValue();

        // 이벤트 연결
        resolutionDropdown.onValueChanged.AddListener(SetResolution);
        // 풀스크린 토글 적용
        fullscreenToggle.isOn = Screen.fullScreen;
        fullscreenToggle.onValueChanged.AddListener(SetFullscreen);

    }

    /// <summary>
    /// 플레이어가 해상도를 선택했을 때 호출
    /// </summary>
    void SetResolution(int index)
    {
        // 중복 제거 후 선택된 index에 맞는 해상도 찾아 적용
        // resolutions에서 직접 찾으면 중복 있는 경우 잘못된 값이 나올 수 있어 아래 방식이 안전함

        string[] selectedText = resolutionDropdown.options[index].text.Split('x');
        int width = int.Parse(selectedText[0].Trim());
        int height = int.Parse(selectedText[1].Trim());

        Screen.SetResolution(width, height, Screen.fullScreen);
    }

    /// <summary>
    /// 전체화면 토글 시 호출
    /// </summary>
    void SetFullscreen(bool isFull)
    {
        Screen.fullScreen = isFull;
    }
}
