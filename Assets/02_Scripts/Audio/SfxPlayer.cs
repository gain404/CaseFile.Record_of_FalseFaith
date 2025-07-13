using System.Collections.Generic;
using UnityEngine;

public class SfxPlayer : MonoBehaviour
{
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioData sfxData;
    
    private Dictionary<AudioName, AudioClip> _clipDictionary = new Dictionary<AudioName, AudioClip>();

    private void Awake()
    {
        foreach (var namedClip in sfxData.clips)
        {
            if (!_clipDictionary.ContainsKey(namedClip.audioName))
            {
                _clipDictionary.Add(namedClip.audioName, namedClip.clip);
            }
        }
    }
    
    public void PlaySound(AudioName audioName)
    {
        if (_clipDictionary.TryGetValue(audioName, out AudioClip clip))
        {
            audioSource.PlayOneShot(clip);
        }
        else
        {
            Debug.LogWarning($"{audioName} 사운드를 찾을 수 없습니다.");
        }
    }
}
