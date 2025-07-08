using System.Collections;
using Unity.Cinemachine;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public static CameraController Instance { get; private set; }

    [SerializeField] private CinemachineCamera virtualCamera;

    private float defaultSize = 10f; //기본 카메라
    private float runningSize = 15f; //달릴 때 카메라
    private float zoomSpeed = 5f; //얼마나 빨리 변할지

    private Coroutine zoomCoroutine;

    private void Awake()
    {
        Instance = this;
        if (virtualCamera == null)
        {
            virtualCamera = FindFirstObjectByType<CinemachineCamera>();
        }
    }


    public void ZoomOutForRunning()
    {
        if (zoomCoroutine != null) StopCoroutine(zoomCoroutine);
        zoomCoroutine = StartCoroutine(ZoomTo(runningSize));
    }

    public void ZoomInToDefault()
    {
        if (zoomCoroutine != null) StopCoroutine(zoomCoroutine);
        zoomCoroutine = StartCoroutine(ZoomTo(defaultSize));
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
}
