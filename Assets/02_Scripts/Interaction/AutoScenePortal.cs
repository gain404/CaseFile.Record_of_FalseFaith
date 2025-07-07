using UnityEngine;
using UnityEngine.SceneManagement;

public class AutoScenePortal : MonoBehaviour
{
    /// <summary>
    /// 씬 이동시켜주는 스크립트와 똑같은데 얘는 자동으로 해줍니다.
    /// </summary>
    
    public string targetSceneName;
    private bool triggered = false;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!triggered && collision.CompareTag("Player"))
        {
            triggered = true;
            SceneManager.LoadScene(targetSceneName);
        }
    }
}
