using System.Collections;
using UnityEngine;

public class UIGameOverEffect : MonoBehaviour
{
    public static UIGameOverEffect Instance { get; private set; }

    public CanvasGroup youDiedPanel; // YouDied 연출을 위한 판넬
    public float fadeDuration = 1.5f;
    public float waitAfterFade = 2f;
    public GameOverManager gameOverManager;


    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void PlayYouDiedEffect()
    {
        StartCoroutine(YouDiedSequence());
    }

    private IEnumerator YouDiedSequence()
    {
        youDiedPanel.gameObject.SetActive(true); //판넬 활성화 - 텍스트 알파값이 0이라 텍스트는 안 보임

        // 페이드 인
        float timer = 0f;
        while (timer < fadeDuration)
        {
            timer += Time.deltaTime;
            youDiedPanel.alpha = Mathf.Lerp(0f, 1f, timer / fadeDuration);
            yield return null;
        }

        // 잠시 멈춤
        yield return new WaitForSeconds(waitAfterFade);

        // 게임 오버 UI 호출
        gameOverManager.ShowGameOver();
    }
}
