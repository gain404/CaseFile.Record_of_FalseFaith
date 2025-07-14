using UnityEngine;

public class PauseManager : MonoBehaviour
{
    /// <summary>
    /// 옵션, 인벤토리 등 사용할 때 일시정지가 필요한 UI는 얘를 넣어주시면 됩니다.
    /// </summary>
    public GameObject pauseUI;
    private bool isPaused = false;


    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (!isPaused)
                Pause();
            else
                Resume();
        }
    }

    public void Pause()
    {
        Time.timeScale = 0f; // 게임 정지
        pauseUI.SetActive(true);
        isPaused = true;
    }

    public void Resume()
    {
        Time.timeScale = 1f; // 게임 재개
        pauseUI.SetActive(false);
        isPaused = false;
    }

    public void QuitGame()
    {
        Time.timeScale = 1f;
        // 씬 이동 or 종료 처리
    }
}
