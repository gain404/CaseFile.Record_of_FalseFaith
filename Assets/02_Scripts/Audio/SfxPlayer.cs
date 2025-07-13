using System.Collections.Generic;
using UnityEngine;

public class SfxPlayer : MonoBehaviour
{
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private SfxData sfxData;
    
    private Dictionary<SfxName, AudioClip> _SfxClipDictionary = new Dictionary<SfxName, AudioClip>();

    private void Awake()
    {
        foreach (var namedClip in sfxData.clips)
        {
            if (!_SfxClipDictionary.ContainsKey(namedClip.audioName))
            {
                _SfxClipDictionary.Add(namedClip.audioName, namedClip.Sfxclip);
            }
        }
    }
    
    public void PlaySfx(SfxName sfxName)
    {
        if (_SfxClipDictionary.TryGetValue(sfxName, out AudioClip clip))
        {
            audioSource.PlayOneShot(clip);
        }
        else
        {
            Debug.LogWarning($"{sfxName} 사운드를 찾을 수 없습니다.");
        }
    }
}
