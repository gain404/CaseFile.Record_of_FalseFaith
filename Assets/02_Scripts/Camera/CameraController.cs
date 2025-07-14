using System.Collections;
using Unity.Cinemachine;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public static CameraController Instance { get; private set; }

    [SerializeField] private CinemachineCamera virtualCamera;

    public float defaultZoomSize = 10f; //기본 카메라
    public float maxZoomOutSize = 15f; //달릴 때 카메라
    private float zoomSpeed = 5f; //얼마나 빨리 변할지

    private float currentDefaultZoom;
    private float currentMaxZoom;

    private Coroutine zoomCoroutine;
    public bool isRunning = false;

    private void Awake()
    {
        Instance = this;
        if (virtualCamera == null)
        {
            virtualCamera = FindFirstObjectByType<CinemachineCamera>();
        }

        // 초기 줌 설정
        currentDefaultZoom = defaultZoomSize;
        currentMaxZoom = maxZoomOutSize;
    }

    public void SetZoomLimits(float newDefault, float newMax)
    {
        currentDefaultZoom = newDefault;
        currentMaxZoom = newMax;

        // 현재 줌 상태에 따라 재적용
        if (isRunning)
            ZoomOutForRunning();
        else
            ZoomInToDefault();
    }

    public void ResetZoomLimits()
    {
        currentDefaultZoom = defaultZoomSize;
        currentMaxZoom = maxZoomOutSize;

        // 현재 줌 상태에 따라 재적용
        if (isRunning)
            ZoomOutForRunning();
        else
            ZoomInToDefault();
    }

    public void ZoomOutForRunning()
    {
        float target = currentMaxZoom;
        StartZoom(target);
    }

    public void ZoomInToDefault()
    {
        StartZoom(currentDefaultZoom);
    }

    private void StartZoom(float targetSize)
    {
        if (zoomCoroutine != null)
        {
            StopCoroutine(zoomCoroutine);
            zoomCoroutine = null;
        }
        zoomCoroutine = StartCoroutine(ZoomTo(targetSize));
    }

    private IEnumerator ZoomTo(float targetSize)
    {
        float initialSize = virtualCamera.Lens.OrthographicSize;
        Vector3 initialPosition = virtualCamera.transform.position;

        while (Mathf.Abs(virtualCamera.Lens.OrthographicSize - targetSize) > 0.01f)
        {
            float currentSize = virtualCamera.Lens.OrthographicSize;
            currentSize = Mathf.Lerp(currentSize, targetSize, Time.deltaTime * zoomSpeed);
            virtualCamera.Lens.OrthographicSize = currentSize;

            // 중심 기준 보정 (줌 시 아래로 내리기)
            float offsetRatio = (currentSize - initialSize) / initialSize;
            Vector3 offset = Vector3.down * offsetRatio;

            virtualCamera.transform.position = initialPosition + offset;

            yield return null;
        }

        virtualCamera.Lens.OrthographicSize = targetSize;
    }

    public void DisableZoomTemporarily()
    {
        currentDefaultZoom = defaultZoomSize;
        currentMaxZoom = defaultZoomSize;
    }
}
