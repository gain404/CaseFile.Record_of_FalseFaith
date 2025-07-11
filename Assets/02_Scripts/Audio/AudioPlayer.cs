using System.Collections.Generic;
using UnityEngine;

public class AudioPlayer : MonoBehaviour
{
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioData audioData;
    
    private Dictionary<string, AudioClip> _clipDictionary = new Dictionary<string, AudioClip>();

    private void Awake()
    {
        foreach (var namedClip in audioData.clips)
        {
            if (!_clipDictionary.ContainsKey(namedClip.name))
            {
                _clipDictionary.Add(namedClip.name, namedClip.clip);
            }
        }
    }
    
    public void PlaySound(string clipName)
    {
        if (_clipDictionary.TryGetValue(clipName, out AudioClip clip))
        {
            audioSource.PlayOneShot(clip);
        }
        else
        {
            Debug.LogWarning($"{clipName} 사운드를 찾을 수 없습니다.");
        }
    }
}
