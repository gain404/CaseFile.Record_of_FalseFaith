using TMPro;
using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.UI;
public class DisplaySetting : MonoBehaviour
{
    public TMP_Dropdown resolutionDropdown;
    public Toggle fullscreenToggle;
    private Resolution[] resolutions;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        resolutions = Screen.resolutions;//스크린 해상도
        resolutionDropdown.ClearOptions();

        int current = 0;
        for (int i = 0; i < resolutions.Length; i++)
        {
            var option = resolutions[i].width + " x " + resolutions[i].height;//유저가 옵션에서 볼 텍스트
            resolutionDropdown.options.Add(new TMP_Dropdown.OptionData(option));

            if (resolutions[i].width == Screen.currentResolution.width &&
                resolutions[i].height == Screen.currentResolution.height)
            {
                current = i;//지금 해상도와 맞는 순서 찾음 보통은(1920*1080)에 해당하는 인덱스가 들어갈 겁니다.
            }
        }
        //해상도 옵션 바꿨으면 적용시켜주기
        resolutionDropdown.value = current;
        resolutionDropdown.onValueChanged.AddListener(SetResolution);
        //풀스크린 옵션 켜져있으면 키기
        fullscreenToggle.isOn = Screen.fullScreen;
        fullscreenToggle.onValueChanged.AddListener(SetFullscreen);

    }

    void SetResolution(int index)
    {
        Resolution res = resolutions[index];
        Screen.SetResolution(res.width, res.height, Screen.fullScreen);
    }

    void SetFullscreen(bool isFull)
    {
        Screen.fullScreen = isFull;
    }

}
