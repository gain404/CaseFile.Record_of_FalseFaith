using System.Collections.Generic;
using UnityEngine;

public class SfxPlayer : MonoBehaviour
{
    [SerializeField] private SfxData sfxData;
    
    private Dictionary<SfxName, AudioClip> _sfxClipDictionary = new Dictionary<SfxName, AudioClip>();
    private AudioSource _audioSource;
    

    private void Awake()
    {
        _audioSource = GetComponent<AudioSource>();
        foreach (var namedClip in sfxData.clips)
        {
            if (!_sfxClipDictionary.ContainsKey(namedClip.audioName))
            {
                _sfxClipDictionary.Add(namedClip.audioName, namedClip.Sfxclip);
            }
        }
    }
    
    public void PlaySfx(SfxName sfxName)
    {
        if (_sfxClipDictionary.TryGetValue(sfxName, out AudioClip clip))
        {
            _audioSource.PlayOneShot(clip);
        }
    }
}
