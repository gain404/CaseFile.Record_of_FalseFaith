using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class AudioSetting : MonoBehaviour
{

    public AudioMixer audioMixer;
    public Slider masterSlider, bgmSlider, sfxSlider;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        masterSlider.onValueChanged.AddListener((value) => SetVolume("Master", value));
        bgmSlider.onValueChanged.AddListener((value) => SetVolume("BGM", value));
        sfxSlider.onValueChanged.AddListener((value) => SetVolume("SFX", value));
    }

    void SetVolume(string name, float value)
    {
        // 오디오 믹서의 값은 -80 ~ 0까지이기 때문에 0.0001 ~ 1의 Log10 * 20을 해줌
        audioMixer.SetFloat(name, Mathf.Log10(value) * 20); // 0~1 -> dB
    }

}
