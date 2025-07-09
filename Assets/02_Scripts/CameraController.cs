using System.Collections;
using Unity.Cinemachine;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public static CameraController Instance { get; private set; }

    [SerializeField] private CinemachineCamera virtualCamera;

    private float defaultZoomSize = 10f; //기본 카메라
    private float maxZoomOutSize = 15f; //달릴 때 카메라
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
        if (zoomCoroutine != null) StopCoroutine(zoomCoroutine);
        zoomCoroutine = StartCoroutine(ZoomTo(targetSize));
    }

    private IEnumerator ZoomTo(float targetSize)
    {
        float currentSize = virtualCamera.Lens.OrthographicSize;

        while (Mathf.Abs(currentSize - targetSize) > 0.01f)
        {
            currentSize = Mathf.Lerp(currentSize, targetSize, Time.deltaTime * zoomSpeed);
            virtualCamera.Lens.OrthographicSize = currentSize;
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
