using UnityEngine;

public class SceneBGMSetter : MonoBehaviour
{
    [Header("이 씬에서 재생할 BGM")]
    public AudioClip sceneBGM;

    private void Start()
    {
        if (sceneBGM != null)
        {
            SoundManager.Instance.SetAndPlayDefaultBGM(sceneBGM);
        }
    }
}
