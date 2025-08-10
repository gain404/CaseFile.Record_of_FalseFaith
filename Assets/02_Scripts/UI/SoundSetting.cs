using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;
using TMPro;
using System;

/// <summary>
/// 게임 사운드 설정을 관리하는 클래스
/// 마스터 볼륨, 배경 음악, 효과음 볼륨을 조절하고 설정을 저장/로드함
/// </summary>
public class SoundSetting : BaseUI
{
    #region Constants
    private const float MIN_VOLUME = 0.0001f;      // 최소 볼륨 (로그 계산을 위한 최소값)
    private const float MAX_VOLUME = 1f;           // 최대 볼륨
    private const float DB_MULTIPLIER = 20f;       // dB 변환 승수
    private const float DEFAULT_VOLUME = 0.8f;     // 기본 볼륨 (80%)
    private const float MUTE_VOLUME = 0f;          // 음소거 볼륨
    #endregion

    #region Serialized Fields
    [Header("Audio System")]
    [SerializeField] private AudioMixer audioMixer;

    [Header("Volume Sliders")]
    [SerializeField] private Slider masterVolumeSlider;
    [SerializeField] private Slider backgroundMusicSlider;
    [SerializeField] private Slider soundEffectSlider;

    [Header("Mute Buttons (Optional)")]
    [SerializeField] private Button masterMuteButton;
    [SerializeField] private Button bgmMuteButton;
    [SerializeField] private Button sfxMuteButton;

    [Header("Volume Display Labels (Optional)")]
    [SerializeField] private TextMeshProUGUI masterVolumeLabel;
    [SerializeField] private TextMeshProUGUI bgmVolumeLabel;
    [SerializeField] private TextMeshProUGUI sfxVolumeLabel;
    #endregion

    #region Private Fields
    // 초기화 상태 추적
    private bool isInitialized = false;

    // 음소거 상태 추적
    private bool isMasterMuted = false;
    private bool isBGMMuted = false;
    private bool isSFXMuted = false;

    // 음소거 이전 볼륨 저장
    private float lastMasterVolume = DEFAULT_VOLUME;
    private float lastBGMVolume = DEFAULT_VOLUME;
    private float lastSFXVolume = DEFAULT_VOLUME;

    // 볼륨 채널 정보
    [System.Serializable]
    private class VolumeChannel
    {
        public string mixerParameter;
        public string prefsKey;
        public Slider slider;
        public Button muteButton;
        public TextMeshProUGUI volumeLabel;

        public VolumeChannel(string mixerParam, string prefsKey, Slider slider, Button muteBtn = null, TextMeshProUGUI label = null)
        {
            this.mixerParameter = mixerParam;
            this.prefsKey = prefsKey;
            this.slider = slider;
            this.muteButton = muteBtn;
            this.volumeLabel = label;
        }
    }

    private VolumeChannel[] volumeChannels;
    #endregion

    #region Unity Lifecycle
    /// <summary>
    /// 컴포넌트 초기화 및 유효성 검사
    /// </summary>
    private void Awake()
    {
        ValidateComponents();
        InitializeVolumeChannels();
    }

    /// <summary>
    /// GameObject가 활성화될 때 자동으로 초기화
    /// </summary>
    private void Start()
    {
        // BaseUI의 InitializePanel이 자동으로 호출되지 않는 경우를 대비
        if (!isInitialized)
        {
            // Debug.Log("SoundSetting: 자동 초기화 실행");
            InitializePanel();
        }
    }

    /// <summary>
    /// GameObject가 활성화될 때마다 호출
    /// </summary>
    private void OnEnable()
    {
        // 이미 초기화된 경우 이벤트만 다시 연결
        if (isInitialized && volumeChannels != null)
        {
            Debug.Log("SoundSetting: OnEnable에서 이벤트 재연결");
            SubscribeToEvents();
        }
    }

    /// <summary>
    /// GameObject가 비활성화될 때 이벤트 해제
    /// </summary>
    private void OnDisable()
    {
        UnsubscribeFromEvents();
    }

    /// <summary>
    /// 이벤트 리스너 해제 및 설정 저장
    /// </summary>
    private void OnDestroy()
    {
        UnsubscribeFromEvents();
        SaveSettings(); // 종료 시 자동 저장
    }
    #endregion

    #region BaseUI Overrides
    /// <summary>
    /// 패널 초기화 - 설정 로드 및 이벤트 연결
    /// </summary>
    public override void InitializePanel()
    {
        if (isInitialized)
        {
            Debug.Log("SoundSetting: 이미 초기화됨");
            return;
        }

        base.InitializePanel();

        if (!ValidateAudioMixer()) return;

        LoadSettings();
        SubscribeToEvents();
        UpdateAllVolumeDisplays();

        isInitialized = true;
        // Debug.Log("SoundSetting 패널이 성공적으로 초기화되었습니다.");
    }

    /// <summary>
    /// UI 시스템 매니저와의 의존성 설정
    /// </summary>
    public override void SetupPanelDependencies(UISystemManager manager)
    {
        // 추후 오디오 매니저나 다른 시스템과 연결 시 사용
        base.SetupPanelDependencies(manager);
    }
    #endregion

    #region Initialization
    /// <summary>
    /// 필수 컴포넌트들의 유효성 검사
    /// </summary>
    private void ValidateComponents()
    {
        if (audioMixer == null)
        {
            Debug.LogError($"[{name}] AudioMixer가 할당되지 않았습니다!");
        }

        if (masterVolumeSlider == null)
            Debug.LogError($"[{name}] Master Volume Slider가 할당되지 않았습니다!");

        if (backgroundMusicSlider == null)
            Debug.LogError($"[{name}] Background Music Slider가 할당되지 않았습니다!");

        if (soundEffectSlider == null)
            Debug.LogError($"[{name}] Sound Effect Slider가 할당되지 않았습니다!");
    }

    /// <summary>
    /// 볼륨 채널 배열 초기화
    /// </summary>
    private void InitializeVolumeChannels()
    {
        volumeChannels = new VolumeChannel[]
        {
            new("Master", "MasterVolume", masterVolumeSlider, masterMuteButton, masterVolumeLabel),
            new("BGM", "BGMVolume", backgroundMusicSlider, bgmMuteButton, bgmVolumeLabel),
            new("SFX", "SFXVolume", soundEffectSlider, sfxMuteButton, sfxVolumeLabel)
        };
    }

    /// <summary>
    /// AudioMixer 유효성 검사 및 파라미터 확인
    /// </summary>
    private bool ValidateAudioMixer()
    {
        if (audioMixer == null)
        {
            Debug.LogError("AudioMixer가 할당되지 않았습니다.");
            return false;
        }

        // AudioMixer 파라미터 존재 여부 확인
        foreach (var channel in volumeChannels)
        {
            if (!audioMixer.GetFloat(channel.mixerParameter, out float testValue))
            {
                Debug.LogError($"AudioMixer에서 '{channel.mixerParameter}' 파라미터를 찾을 수 없습니다!");
                Debug.LogError("AudioMixer Window에서 해당 파라미터를 우클릭 → 'Expose Parameter'를 선택하세요.");
                return false;
            }
            else
            {
                Debug.Log($"AudioMixer 파라미터 '{channel.mixerParameter}' 확인됨. 현재 값: {testValue}dB");
            }
        }

        return true;
    }
    #endregion

    #region Event Management
    /// <summary>
    /// 모든 이벤트 리스너 등록
    /// </summary>
    private void SubscribeToEvents()
    {
        // 기존 이벤트 먼저 해제 (중복 방지)
        UnsubscribeFromEvents();

        // 슬라이더 이벤트 등록
        if (masterVolumeSlider != null)
        {
            masterVolumeSlider.onValueChanged.AddListener(OnMasterVolumeChanged);
            // Debug.Log("Master 슬라이더 이벤트 연결됨");
        }

        if (backgroundMusicSlider != null)
        {
            backgroundMusicSlider.onValueChanged.AddListener(OnBGMVolumeChanged);
            // Debug.Log("BGM 슬라이더 이벤트 연결됨");
        }

        if (soundEffectSlider != null)
        {
            soundEffectSlider.onValueChanged.AddListener(OnSFXVolumeChanged);
            // Debug.Log("SFX 슬라이더 이벤트 연결됨");
        }

        // 음소거 버튼 이벤트 등록
        if (masterMuteButton != null)
            masterMuteButton.onClick.AddListener(() => ToggleMute("Master"));

        if (bgmMuteButton != null)
            bgmMuteButton.onClick.AddListener(() => ToggleMute("BGM"));

        if (sfxMuteButton != null)
            sfxMuteButton.onClick.AddListener(() => ToggleMute("SFX"));
    }

    /// <summary>
    /// 모든 이벤트 리스너 해제
    /// </summary>
    private void UnsubscribeFromEvents()
    {
        // 슬라이더 이벤트 해제
        if (masterVolumeSlider != null)
            masterVolumeSlider.onValueChanged.RemoveListener(OnMasterVolumeChanged);

        if (backgroundMusicSlider != null)
            backgroundMusicSlider.onValueChanged.RemoveListener(OnBGMVolumeChanged);

        if (soundEffectSlider != null)
            soundEffectSlider.onValueChanged.RemoveListener(OnSFXVolumeChanged);

        // 음소거 버튼 이벤트 해제
        if (masterMuteButton != null)
            masterMuteButton.onClick.RemoveAllListeners();

        if (bgmMuteButton != null)
            bgmMuteButton.onClick.RemoveAllListeners();

        if (sfxMuteButton != null)
            sfxMuteButton.onClick.RemoveAllListeners();
    }
    #endregion

    #region Volume Control Methods
    /// <summary>
    /// 마스터 볼륨 변경 이벤트 핸들러
    /// </summary>
    private void OnMasterVolumeChanged(float value)
    {
        // Debug.Log($"마스터 볼륨 변경: {value}");
        SetVolume("Master", value);
        isMasterMuted = (value <= MIN_VOLUME);
        lastMasterVolume = isMasterMuted ? lastMasterVolume : value;
        UpdateVolumeDisplay(masterVolumeLabel, value);
        UpdateMuteButtonState(masterMuteButton, isMasterMuted);
    }

    /// <summary>
    /// 배경음 볼륨 변경 이벤트 핸들러
    /// </summary>
    private void OnBGMVolumeChanged(float value)
    {
        // Debug.Log($"BGM 볼륨 변경: {value}");
        SetVolume("BGM", value);
        isBGMMuted = (value <= MIN_VOLUME);
        lastBGMVolume = isBGMMuted ? lastBGMVolume : value;
        UpdateVolumeDisplay(bgmVolumeLabel, value);
        UpdateMuteButtonState(bgmMuteButton, isBGMMuted);
    }

    /// <summary>
    /// 효과음 볼륨 변경 이벤트 핸들러
    /// </summary>
    private void OnSFXVolumeChanged(float value)
    {
        // Debug.Log($"SFX 볼륨 변경: {value}");
        SetVolume("SFX", value);
        isSFXMuted = (value <= MIN_VOLUME);
        lastSFXVolume = isSFXMuted ? lastSFXVolume : value;
        UpdateVolumeDisplay(sfxVolumeLabel, value);
        UpdateMuteButtonState(sfxMuteButton, isSFXMuted);
    }

    /// <summary>
    /// 볼륨 값을 AudioMixer에 적용하고 설정 저장
    /// </summary>
    private void SetVolume(string parameterName, float linearValue)
    {
        if (audioMixer == null)
        {
            Debug.LogError("AudioMixer가 null입니다!");
            return;
        }

        // 선형 값을 dB로 변환 (0-1 → -80dB ~ 0dB)
        float clampedValue = Mathf.Clamp(linearValue, MIN_VOLUME, MAX_VOLUME);
        float dbValue = Mathf.Log10(clampedValue) * DB_MULTIPLIER;

        // AudioMixer에 적용 - 성공 여부 확인
        bool success = audioMixer.SetFloat(parameterName, dbValue);
        if (!success)
        {
            Debug.LogError($"AudioMixer 파라미터 '{parameterName}' 설정에 실패했습니다!");
            Debug.LogError("AudioMixer에서 해당 파라미터가 Exposed되어 있는지 확인하세요.");
            return;
        }

        // Debug.Log($"{parameterName} 볼륨 설정: {linearValue:F2} ({dbValue:F1}dB)");

        // 즉시 저장 (자동 저장)
        PlayerPrefs.SetFloat($"{parameterName}Volume", linearValue);
        PlayerPrefs.Save();
    }

    /// <summary>
    /// 음소거 토글 기능
    /// </summary>
    private void ToggleMute(string channelType)
    {
        switch (channelType)
        {
            case "Master":
                if (isMasterMuted)
                {
                    masterVolumeSlider.value = lastMasterVolume;
                    isMasterMuted = false;
                }
                else
                {
                    lastMasterVolume = masterVolumeSlider.value;
                    masterVolumeSlider.value = MUTE_VOLUME;
                    isMasterMuted = true;
                }
                break;

            case "BGM":
                if (isBGMMuted)
                {
                    backgroundMusicSlider.value = lastBGMVolume;
                    isBGMMuted = false;
                }
                else
                {
                    lastBGMVolume = backgroundMusicSlider.value;
                    backgroundMusicSlider.value = MUTE_VOLUME;
                    isBGMMuted = true;
                }
                break;

            case "SFX":
                if (isSFXMuted)
                {
                    soundEffectSlider.value = lastSFXVolume;
                    isSFXMuted = false;
                }
                else
                {
                    lastSFXVolume = soundEffectSlider.value;
                    soundEffectSlider.value = MUTE_VOLUME;
                    isSFXMuted = true;
                }
                break;
        }
    }
    #endregion

    #region Settings Management
    /// <summary>
    /// 모든 볼륨 설정을 저장
    /// </summary>
    public void SaveSettings()
    {
        try
        {
            foreach (var channel in volumeChannels)
            {
                if (channel.slider != null)
                {
                    PlayerPrefs.SetFloat(channel.prefsKey, channel.slider.value);
                }
            }

            // 음소거 상태도 저장
            PlayerPrefs.SetInt("MasterMuted", isMasterMuted ? 1 : 0);
            PlayerPrefs.SetInt("BGMMuted", isBGMMuted ? 1 : 0);
            PlayerPrefs.SetInt("SFXMuted", isSFXMuted ? 1 : 0);

            PlayerPrefs.Save();
            // Debug.Log("사운드 설정이 저장되었습니다.");
        }
        catch (Exception e)
        {
            Debug.LogError($"사운드 설정 저장 중 오류 발생: {e.Message}");
        }
    }

    /// <summary>
    /// 저장된 볼륨 설정을 로드
    /// </summary>
    public void LoadSettings()
    {
        try
        {
            foreach (var channel in volumeChannels)
            {
                if (channel.slider != null)
                {
                    float savedValue = PlayerPrefs.GetFloat(channel.prefsKey, DEFAULT_VOLUME);
                    channel.slider.value = savedValue;

                    // AudioMixer에도 적용
                    SetVolumeDirectly(channel.mixerParameter, savedValue);
                }
            }

            // 음소거 상태 로드
            isMasterMuted = PlayerPrefs.GetInt("MasterMuted", 0) == 1;
            isBGMMuted = PlayerPrefs.GetInt("BGMMuted", 0) == 1;
            isSFXMuted = PlayerPrefs.GetInt("SFXMuted", 0) == 1;

            UpdateAllMuteButtonStates();
            // Debug.Log("사운드 설정이 로드되었습니다.");
        }
        catch (Exception e)
        {
            Debug.LogError($"사운드 설정 로드 중 오류 발생: {e.Message}");
            ResetToDefaults();
        }
    }

    /// <summary>
    /// 설정 저장 없이 AudioMixer에만 볼륨 적용 (로드 시 사용)
    /// </summary>
    private void SetVolumeDirectly(string parameterName, float linearValue)
    {
        if (audioMixer == null) return;

        float clampedValue = Mathf.Clamp(linearValue, MIN_VOLUME, MAX_VOLUME);
        float dbValue = Mathf.Log10(clampedValue) * DB_MULTIPLIER;
        audioMixer.SetFloat(parameterName, dbValue);
    }

    /// <summary>
    /// 모든 설정을 기본값으로 리셋
    /// </summary>
    public void ResetToDefaults()
    {
        if (masterVolumeSlider != null) masterVolumeSlider.value = DEFAULT_VOLUME;
        if (backgroundMusicSlider != null) backgroundMusicSlider.value = DEFAULT_VOLUME;
        if (soundEffectSlider != null) soundEffectSlider.value = DEFAULT_VOLUME;

        isMasterMuted = false;
        isBGMMuted = false;
        isSFXMuted = false;

        UpdateAllVolumeDisplays();
        UpdateAllMuteButtonStates();

        // Debug.Log("사운드 설정이 기본값으로 리셋되었습니다.");
    }
    #endregion

    #region UI Updates
    /// <summary>
    /// 볼륨 표시 라벨 업데이트 (백분율로 표시)
    /// </summary>
    private void UpdateVolumeDisplay(TextMeshProUGUI label, float value)
    {
        if (label != null)
        {
            int percentage = Mathf.RoundToInt(value * 100f);
            label.text = $"{percentage}%";
            // Debug.Log($"라벨 업데이트: {percentage}%");
        }
        else
        {
            Debug.LogWarning("볼륨 라벨이 null입니다!");
        }
    }

    /// <summary>
    /// 음소거 버튼 상태 업데이트
    /// </summary>
    private void UpdateMuteButtonState(Button muteButton, bool isMuted)
    {
        if (muteButton != null)
        {
            // 버튼 색상이나 텍스트 변경 (프로젝트에 맞게 수정)
            var colors = muteButton.colors;
            colors.normalColor = isMuted ? Color.red : Color.white;
            muteButton.colors = colors;
        }
    }

    /// <summary>
    /// 모든 볼륨 표시 라벨 업데이트
    /// </summary>
    private void UpdateAllVolumeDisplays()
    {
        if (masterVolumeSlider != null)
            UpdateVolumeDisplay(masterVolumeLabel, masterVolumeSlider.value);

        if (backgroundMusicSlider != null)
            UpdateVolumeDisplay(bgmVolumeLabel, backgroundMusicSlider.value);

        if (soundEffectSlider != null)
            UpdateVolumeDisplay(sfxVolumeLabel, soundEffectSlider.value);
    }

    /// <summary>
    /// 모든 음소거 버튼 상태 업데이트
    /// </summary>
    private void UpdateAllMuteButtonStates()
    {
        UpdateMuteButtonState(masterMuteButton, isMasterMuted);
        UpdateMuteButtonState(bgmMuteButton, isBGMMuted);
        UpdateMuteButtonState(sfxMuteButton, isSFXMuted);
    }
    #endregion

    #region Public API
    /// <summary>
    /// 외부에서 마스터 볼륨을 설정할 때 사용
    /// </summary>
    public void SetMasterVolumeExternal(float value)
    {
        if (masterVolumeSlider != null)
        {
            masterVolumeSlider.value = Mathf.Clamp(value, 0f, 1f);
        }
    }

    /// <summary>
    /// 외부에서 BGM 볼륨을 설정할 때 사용
    /// </summary>
    public void SetBGMVolumeExternal(float value)
    {
        if (backgroundMusicSlider != null)
        {
            backgroundMusicSlider.value = Mathf.Clamp(value, 0f, 1f);
        }
    }

    /// <summary>
    /// 외부에서 SFX 볼륨을 설정할 때 사용
    /// </summary>
    public void SetSFXVolumeExternal(float value)
    {
        if (soundEffectSlider != null)
        {
            soundEffectSlider.value = Mathf.Clamp(value, 0f, 1f);
        }
    }

    /// <summary>
    /// 현재 마스터 볼륨 값 반환
    /// </summary>
    public float GetMasterVolume()
    {
        return masterVolumeSlider != null ? masterVolumeSlider.value : DEFAULT_VOLUME;
    }

    /// <summary>
    /// 현재 BGM 볼륨 값 반환
    /// </summary>
    public float GetBGMVolume()
    {
        return backgroundMusicSlider != null ? backgroundMusicSlider.value : DEFAULT_VOLUME;
    }

    /// <summary>
    /// 현재 SFX 볼륨 값 반환
    /// </summary>
    public float GetSFXVolume()
    {
        return soundEffectSlider != null ? soundEffectSlider.value : DEFAULT_VOLUME;
    }
    #endregion
}