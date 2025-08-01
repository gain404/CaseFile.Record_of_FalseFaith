using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ObjectiveUIItem : MonoBehaviour
{
    [Header("UI Components")]
    [SerializeField] private TextMeshProUGUI objectiveText;// 퀘스트 설명 텍스트
    [SerializeField] private TextMeshProUGUI progressText;// 진행도 텍스트 (예: 3/5)
    [SerializeField] private Image checkmarkIcon; // 체크 표시
    [SerializeField] private GameObject strikethroughLine; // 취소선 이미지

    [Header("Visual Settings")]
    [SerializeField] private Color activeColor = Color.white;
    [SerializeField] private Color completedTextColor = new Color(0.0f, 0.0f, 0.0f, 1f);
    [SerializeField] private Color completedBackgroundColor = new Color(0.2f, 0.7f, 0.2f, 0.3f); //완료시 좀 더 어둡게
    [SerializeField] private Color progressColor = Color.yellow;

    [Header("Animation Settings")]
    [SerializeField] private float completionAnimationDuration = 0.5f;
    [SerializeField] private float strikethroughAnimationDuration = 0.3f;

    private ObjectiveData currentObjective;

    /// <summary>
    /// 외부에서 ObjectiveData를 전달받아 UI 항목 초기화
    /// </summary>
    public void Setup(ObjectiveData objective)
    {
        currentObjective = objective;
        objectiveText.text = objective.content;

        // 초기 상태 설정
        ResetVisualState();

        if (objective.achieve)
        {
            MarkAsCompleted(false); // 애니메이션 없이 완료 상태로
        }
        else
        {
            UpdateProgress(objective);
        }
    }
    /// <summary>
    /// 기본 상태로 시각 효과 초기화
    /// </summary>
    private void ResetVisualState()
    {
        if (objectiveText != null)
            objectiveText.color = activeColor;

        if (checkmarkIcon != null)
            checkmarkIcon.gameObject.SetActive(false);

        if (progressText != null)
            progressText.color = progressColor;

        if (strikethroughLine != null)
            strikethroughLine.SetActive(false);
    }



    /// <summary>
    /// 아이템 수집, 몬스터 처치 등 진행도를 UI에 표시
    /// </summary>
    public void UpdateProgress(ObjectiveData objective)
    {
        if (progressText == null) return;

        // 진행도 텍스트 업데이트
        if (objective.targetCount > 1)
        {
            progressText.text = $"({objective.currentCount}/{objective.targetCount})";
            progressText.gameObject.SetActive(true);
        }
        else
        {
            // 목표 개수가 1이면 진행도 텍스트를 숨김
            progressText.gameObject.SetActive(false);
        }
    }

    /// <summary>
    /// 퀘스트 완료 시 UI 처리. withAnimation=true면 애니메이션 재생
    /// </summary>
    public void MarkAsCompleted(bool withAnimation = true)
    {
        Debug.Log($"MarkAsCompleted 호출됨: {currentObjective?.content}, 애니메이션: {withAnimation}");

        if (withAnimation)
        {
            StartCoroutine(CompletionAnimationSequence());
        }
        else
        {
            // 즉시 완료 상태로 변경
            ApplyCompletedVisuals();
        }
    }

    private void ApplyCompletedVisuals()
    {
        Debug.Log("완료 비주얼 적용 시작");

        // 텍스트 색상 변경
        if (objectiveText != null)
        {
            objectiveText.color = completedTextColor;
            Debug.Log("텍스트 색상 변경 완료");
        }
        else
        {
            Debug.LogWarning("objectiveText가 null입니다!");
        }

        // 체크마크 표시
        if (checkmarkIcon != null)
        {
            checkmarkIcon.gameObject.SetActive(true);
            Debug.Log("체크마크 활성화 완료");
        }
        else
        {
            Debug.LogWarning("checkmarkIcon이 null입니다!");
        }

        // 진행도 텍스트 숨기기
        if (progressText != null)
        {
            progressText.gameObject.SetActive(false);
        }

        Debug.Log("완료 비주얼 적용 완료");
    }

    private IEnumerator CompletionAnimationSequence()
    {
        // 1단계: 확대 애니메이션
        yield return StartCoroutine(ScaleAnimation(1f, 1.1f, 0.2f));

        // 2단계: 체크마크 표시
        checkmarkIcon.gameObject.SetActive(true);
        yield return new WaitForSeconds(0.1f);

        // 3단계: 취소선 애니메이션
        yield return StartCoroutine(ShowStrikethroughAnimation());

        // 4단계: 텍스트 색상 변경
        yield return StartCoroutine(ColorChangeAnimation());

        // 5단계: 원래 크기로 복원
        yield return StartCoroutine(ScaleAnimation(1.1f, 1f, 0.2f));

        // 진행도 텍스트 숨기기
        progressText.gameObject.SetActive(false);
    }
    /// <summary>
    /// UI 항목을 커졌다가 작아지게 만드는 확대/축소 애니메이션
    /// </summary>
    private IEnumerator ScaleAnimation(float startScale, float endScale, float duration)
    {
        float timer = 0;
        while (timer < duration)
        {
            timer += Time.deltaTime;
            float scale = Mathf.Lerp(startScale, endScale, timer / duration);
            transform.localScale = Vector3.one * scale;
            yield return null;
        }
        transform.localScale = Vector3.one * endScale;
    }

    private IEnumerator ShowStrikethroughAnimation()
    {
        strikethroughLine.SetActive(true);
        // 취소선 긋는 효과음
        SoundManager.Instance.PlayObjectiveGet();
        // 취소선을 왼쪽에서 오른쪽으로 그리는 애니메이션
        RectTransform strikeRect = strikethroughLine.GetComponent<RectTransform>();
        Vector2 originalSize = strikeRect.sizeDelta;

        strikeRect.sizeDelta = new Vector2(0, originalSize.y);

        float timer = 0;
        while (timer < strikethroughAnimationDuration)
        {
            timer += Time.deltaTime;
            float width = Mathf.Lerp(0, originalSize.x, timer / strikethroughAnimationDuration);
            strikeRect.sizeDelta = new Vector2(width, originalSize.y);
            yield return null;
        }

        strikeRect.sizeDelta = originalSize;
    }

    private IEnumerator ColorChangeAnimation()
    {
        Color startColor = objectiveText.color;
        float timer = 0;

        while (timer < 0.3f)
        {
            timer += Time.deltaTime;
            objectiveText.color = Color.Lerp(startColor, completedTextColor, timer / 0.3f);

            yield return null;
        }

        objectiveText.color = completedTextColor;

    }

    private IEnumerator ProgressUpdateAnimation()
    {
        // 진행도 업데이트 시 살짝 깜빡이는 효과
        Color originalColor = progressText.color;

        for (int i = 0; i < 2; i++)
        {
            progressText.color = Color.white;
            yield return new WaitForSeconds(0.1f);
            progressText.color = originalColor;
            yield return new WaitForSeconds(0.1f);
        }
    }
}
