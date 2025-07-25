using UnityEngine;

public class LocationTrigger : MonoBehaviour
{
    [Header("위치 설정")]
    [SerializeField] private string locationId = "loc_restaurant"; // CSV의 targetId와 일치
    [SerializeField] private string locationName = "국밥집";

    [Header("시각적 설정")]
    [SerializeField] private bool showGizmo = true;
    [SerializeField] private Color gizmoColor = Color.green;
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private Color highlightColor = Color.green;

    [Header("트리거 설정")]
    [SerializeField] private bool onlyTriggerOnce = true; // 한 번만 트리거할지 여부

    private Color originalColor;
    private bool hasTriggered = false;

    private void Start()
    {
        // 기본 스프라이트 설정이 없다면 간단한 사각형 만들기
        if (spriteRenderer == null)
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
        }

        if (spriteRenderer != null)
        {
            originalColor = spriteRenderer.color;
        }

        // Collider가 없다면 자동으로 추가
        if (GetComponent<Collider2D>() == null)
        {
            BoxCollider2D collider = gameObject.AddComponent<BoxCollider2D>();
            collider.isTrigger = true;
            collider.size = new Vector2(2f, 2f); // 기본 크기
        }

        Debug.Log($"LocationTrigger 설정 완료: {locationName} (ID: {locationId})");
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            if (onlyTriggerOnce && hasTriggered)
                return;

            ReachLocation();
        }
    }

    private void ReachLocation()
    {
        hasTriggered = true;

        Debug.Log($"플레이어가 위치에 도달했습니다: {locationName} (ID: {locationId})");

        // 목표 매니저에 위치 도달 알림
        if (ObjectiveManager.Instance != null)
        {
            ObjectiveManager.Instance.OnLocationReached(locationId);
        }
        else
        {
            Debug.LogError("ObjectiveManager.Instance가 null입니다!");
        }

        // 도달 효과
        StartCoroutine(LocationReachedEffect());
    }

    private System.Collections.IEnumerator LocationReachedEffect()
    {
        // 색상 변경 효과
        if (spriteRenderer != null)
        {
            Color original = spriteRenderer.color;

            // 깜빡임 효과
            for (int i = 0; i < 3; i++)
            {
                spriteRenderer.color = highlightColor;
                yield return new WaitForSeconds(0.2f);
                spriteRenderer.color = original;
                yield return new WaitForSeconds(0.2f);
            }

            // 최종적으로 하이라이트 색상으로 유지 (도달했음을 표시)
            spriteRenderer.color = highlightColor;
        }
    }

    private void OnDrawGizmos()
    {
        if (showGizmo)
        {
            Gizmos.color = hasTriggered ? Color.blue : gizmoColor;

            // Collider 영역 표시
            Collider2D col = GetComponent<Collider2D>();
            if (col != null)
            {
                Gizmos.DrawWireCube(transform.position, col.bounds.size);
            }
            else
            {
                Gizmos.DrawWireCube(transform.position, new Vector3(2f, 2f, 0f));
            }

            // 위치 이름 표시 (Scene 뷰에서)
            UnityEditor.Handles.Label(transform.position + Vector3.up * 1.5f, locationName);
        }
    }

    // 에디터에서 테스트용
    [ContextMenu("위치 도달 테스트")]
    private void TestReachLocation()
    {
        Debug.Log("수동으로 위치 도달 테스트 실행");
        ReachLocation();
    }

    // 트리거 상태 리셋 (테스트용)
    [ContextMenu("트리거 상태 리셋")]
    private void ResetTrigger()
    {
        hasTriggered = false;
        if (spriteRenderer != null)
        {
            spriteRenderer.color = originalColor;
        }
        Debug.Log($"트리거 상태 리셋: {locationName}");
    }
}
