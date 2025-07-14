using System.Collections.Generic;
using UnityEngine;

public class BgmManager : MonoBehaviour
{
    public static BgmManager Instance { get; private set; }
    
    [SerializeField] private AudioSource bgmSource;
    [SerializeField] private BgmData bgmData;
    
    private Dictionary<BgmName, AudioClip> _BgmClipDictionary = new Dictionary<BgmName, AudioClip>();

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
        
        foreach (var namedClip in bgmData.clips)
        {
            if (!_BgmClipDictionary.ContainsKey(namedClip.bgmName))
            {
                _BgmClipDictionary.Add(namedClip.bgmName, namedClip.bgmClip);
            }
        }
    }

    public void PlaySfx(BgmName bgmName)
    {
        if (bgmSource.isPlaying)
        {
            bgmSource.Stop();
        }
        
        if (_BgmClipDictionary.TryGetValue(bgmName, out AudioClip bgmClip))
        {
            bgmSource.clip = bgmClip;
            bgmSource.Play();
        }
    }
    
    
    public void StopBgm()
    {
        bgmSource.Stop();
    }
    
    
    // 현재 BGM 재생 여부 반환
    public bool IsPlayingBgm()
    {
        return bgmSource.isPlaying;
    }
}
