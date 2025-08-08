using UnityEngine;

public class SceneBGMSetter : MonoBehaviour
{
    [Header("이 씬에서 재생할 BGM")]
    public AudioClip sceneBGM;

    [Header("특정 상황에서 재생할 BGM")]
    public AudioClip triggerBGM;
    
    private void Start()
    {
        if (sceneBGM != null)
        {
            SoundManager.Instance.SetAndPlayDefaultBGM(sceneBGM);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player") && triggerBGM != null)
        {
            SoundManager.Instance.SetAndPlayDefaultBGM(triggerBGM);
        }
    }
}
