using UnityEngine;

public class BgmManager : MonoBehaviour
{
    public static BgmManager Instance { get; private set; }
    
    [SerializeField] private AudioSource bgmSource;
    [SerializeField] private BgmData bgmData;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }
    
    
    public void StopBGM()
    {
        bgmSource.Stop();
    }
    
    
    // 현재 BGM 재생 여부 반환
    public bool IsPlayingBGM()
    {
        return bgmSource.isPlaying;
    }
}
