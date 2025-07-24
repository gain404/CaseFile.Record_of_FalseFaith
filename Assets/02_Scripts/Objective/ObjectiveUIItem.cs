using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ObjectiveUIItem : MonoBehaviour
{
    [Header("UI Components")]
    [SerializeField] private TextMeshProUGUI objectiveText;
    [SerializeField] private TextMeshProUGUI progressText;
    [SerializeField] private Image checkmarkIcon;

    [Header("Visual Settings")]
    [SerializeField] private Color activeColor = Color.white;
    [SerializeField] private Color completedTextColor = new Color(0.7f, 0.7f, 0.7f, 1f);
    [SerializeField] private Color completedBackgroundColor = new Color(0.2f, 0.7f, 0.2f, 0.3f);
    [SerializeField] private Color progressColor = Color.yellow;

    [Header("Animation Settings")]
    [SerializeField] private float completionAnimationDuration = 0.5f;
    [SerializeField] private float strikethroughAnimationDuration = 0.3f;

    private ObjectiveData currentObjective;

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

    private void ResetVisualState()
    {
        if (objectiveText != null)
            objectiveText.color = activeColor;

        if (checkmarkIcon != null)
            checkmarkIcon.gameObject.SetActive(false);

        if (progressText != null)
            progressText.color = progressColor;
    }




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
            progressText.gameObject.SetActive(false);
        }
    }

    public void MarkAsCompleted(bool withAnimation = true)
    {
        Debug.Log($"MarkAsCompleted 호출됨: {currentObjective?.content}, 애니메이션: {withAnimation}");

        // 즉시 완료 상태로 변경 (애니메이션 없이)
        ApplyCompletedVisuals();
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

        // 4단계: 텍스트 색상 변경
        yield return StartCoroutine(ColorChangeAnimation());

        // 5단계: 원래 크기로 복원
        yield return StartCoroutine(ScaleAnimation(1.1f, 1f, 0.2f));

        // 진행도 텍스트 숨기기
        progressText.gameObject.SetActive(false);
    }

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
