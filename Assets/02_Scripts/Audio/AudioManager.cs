using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }
    
    [SerializeField] private AudioSource bgmSource;
    
    [SerializeField] private AudioClip[] bgmClips;

    private void Awake()
    {
        // Singleton 구성
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    /// <summary>
    /// BGM 재생
    /// </summary>
    /// <param name="index">bgmClips 배열 인덱스</param>
    public void PlayBGM(int index)
    {
        if (index < 0 || index >= bgmClips.Length)
        {
            Debug.LogWarning($"BGM index {index} is out of range.");
            return;
        }

        bgmSource.clip = bgmClips[index];
        bgmSource.loop = true;
        bgmSource.Play();
    }

    /// <summary>
    /// BGM 중지
    /// </summary>
    public void StopBGM()
    {
        bgmSource.Stop();
    }

    /// <summary>
    /// BGM 볼륨 설정 (0~1)
    /// </summary>
    public void SetBGMVolume(float volume)
    {
        bgmSource.volume = Mathf.Clamp01(volume);
    }

    /// <summary>
    /// 현재 BGM 재생 여부 반환
    /// </summary>
    public bool IsPlayingBGM()
    {
        return bgmSource.isPlaying;
    }
}
