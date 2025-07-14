using UnityEngine;

public class ZoomZone : MonoBehaviour
{
    /// <summary>
    /// 콜라이더를 붙여준 영역에 들어가는 컴포넌트입니다.
    /// 캐릭터가 달릴 때 줌 기능을 관리합니다.
    /// </summary>
    public float maxZoomOutSize = 15f; // 달릴 때 허용할 최대 줌 아웃 크기
    public float defaultZoomSize = 10f; // 기본 카메라 크기

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            CameraController.Instance.SetZoomLimits(defaultZoomSize, maxZoomOutSize);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            // 기본값으로 복구
            CameraController.Instance.ResetZoomLimits();
        }
    }
}
