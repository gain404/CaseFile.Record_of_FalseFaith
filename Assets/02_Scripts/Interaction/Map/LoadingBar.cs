using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class LoadingBar : MonoBehaviour
{
    public static string nextScene;

    [SerializeField] Image progressBar;

    [Header("Tip Settings")]
    [SerializeField] private TextMeshProUGUI tipText;
    
    // ▼▼▼ 스크립터블 오브젝트 또는 배열 방식 중 사용할 팁 데이터 소스를 선택하세요. ▼▼▼
    // [SerializeField] private TipData tipData; // 스크립터블 오브젝트를 사용할 경우
    [SerializeField] private string[] tips;   // 간단한 배열을 사용할 경우

    // ▼▼▼ 화면 전체 클릭을 감지할 버튼 변수 ▼▼▼
    [SerializeField] private Button clickAreaButton;
    
    private int lastTipIndex = -1;

    // Input System 관련 코드(Awake, OnEnable, OnDisable, Update)는 모두 삭제되었습니다.

    void Start()
    {
        // ▼▼▼ 버튼이 클릭되면 ShowRandomTip 함수를 실행하도록 등록 ▼▼▼
        if (clickAreaButton != null)
        {
            clickAreaButton.onClick.AddListener(ShowRandomTip);
        }
        
        // 시작할 때 첫 팁을 보여줌
        ShowRandomTip();
        
        StartCoroutine(LoadScene());
    }

    void ShowRandomTip()
    {
        // 팁 배열이 비어있으면 아무것도 하지 않음 (스크립터블 오브젝트 사용 시 tipData.tips.Count 로 변경)
        if (tips == null || tips.Length == 0)
        {
            tipText.text = "";
            return;
        }

        int randomIndex;
        do
        {
            // 팁 배열에서 랜덤 인덱스 추출 (스크립터블 오브젝트 사용 시 tipData.tips.Count 로 변경)
            randomIndex = Random.Range(0, tips.Length);
        } while (tips.Length > 1 && randomIndex == lastTipIndex);

        lastTipIndex = randomIndex;
        // 팁 텍스트 설정 (스크립터블 오브젝트 사용 시 tipData.tips[randomIndex] 로 변경)
        tipText.text = tips[randomIndex];
    }

    public static void LoadScene(string sceneName)
    {
        nextScene = sceneName;
        SceneManager.LoadScene("LoadingScene");
    }

    IEnumerator LoadScene()
    {
        yield return null;
        AsyncOperation op = SceneManager.LoadSceneAsync(nextScene);
        op.allowSceneActivation = false;
        float timer = 0.0f;
        while (!op.isDone)
        {
            yield return null;
            timer += Time.deltaTime;
            if (op.progress < 0.9f)
            {
                progressBar.fillAmount = Mathf.Lerp(progressBar.fillAmount, op.progress, timer);
                if (progressBar.fillAmount >= op.progress)
                {
                    timer = 0f;
                }
            }
            else
            {
                progressBar.fillAmount = Mathf.Lerp(progressBar.fillAmount, 1f, timer);
                if (progressBar.fillAmount == 1.0f)
                {
                    op.allowSceneActivation = true;
                    yield break;
                }
            }
        }
    }
}