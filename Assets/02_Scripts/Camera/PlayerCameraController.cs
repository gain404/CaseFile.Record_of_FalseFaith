using System;
using System.Collections;
using Unity.Cinemachine;
using UnityEngine;

public class PlayerCameraController : MonoBehaviour
{
    public bool isRunning;
    
    [SerializeField] private float defaultZoomSize = 10f; //기본 카메라
    [SerializeField] private float maxZoomOutSize = 15f; //달릴 때 카메라
    [SerializeField] private float zoomSpeed = 5f; //얼마나 빨리 변할지

    private float _currentDefaultZoom;
    private float _currentMaxZoom;
    private CinemachineCamera _playerCamera;
    private Coroutine _zoomCoroutine;

    private void Awake()
    {
        _playerCamera = GetComponent<CinemachineCamera>();
        _currentDefaultZoom = defaultZoomSize;
        _currentMaxZoom = maxZoomOutSize;
        isRunning = false;
    }

    private void Start()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        _playerCamera.Follow = player.transform.Find("CameraTarget");
    }

    public void SetZoomLimits(float newDefault, float newMax)
    {
        _currentDefaultZoom = newDefault;
        _currentMaxZoom = newMax;

        // 현재 줌 상태에 따라 재적용
        if (isRunning)
            ZoomOutForRunning();
        else
            ZoomInToDefault();
    }

    public void ResetZoomLimits()
    {
        _currentDefaultZoom = defaultZoomSize;
        _currentMaxZoom = maxZoomOutSize;

        // 현재 줌 상태에 따라 재적용
        if (isRunning)
            ZoomOutForRunning();
        else
            ZoomInToDefault();
    }

    public void ZoomOutForRunning()
    {
        float target = _currentMaxZoom;
        StartZoom(target);
    }

    public void ZoomInToDefault()
    {
        StartZoom(_currentDefaultZoom);
    }

    private void StartZoom(float targetSize)
    {
        if (_zoomCoroutine != null)
        {
            StopCoroutine(_zoomCoroutine);
            _zoomCoroutine = null;
        }
        _zoomCoroutine = StartCoroutine(ZoomTo(targetSize));
    }

    private IEnumerator ZoomTo(float targetSize)
    {
        float initialSize = _playerCamera.Lens.OrthographicSize;
        Vector3 initialPosition = _playerCamera.transform.position;

        while (Mathf.Abs(_playerCamera.Lens.OrthographicSize - targetSize) > 0.01f)
        {
            float currentSize = _playerCamera.Lens.OrthographicSize;
            currentSize = Mathf.Lerp(currentSize, targetSize, Time.deltaTime * zoomSpeed);
            _playerCamera.Lens.OrthographicSize = currentSize;

            // 중심 기준 보정 (줌 시 아래로 내리기)
            float offsetRatio = (currentSize - initialSize) / initialSize;
            Vector3 offset = Vector3.down * offsetRatio;

            _playerCamera.transform.position = initialPosition + offset;

            yield return null;
        }

        _playerCamera.Lens.OrthographicSize = targetSize;
    }

    public void DisableZoomTemporarily()
    {
        _currentDefaultZoom = defaultZoomSize;
        _currentMaxZoom = defaultZoomSize;
    }
}
