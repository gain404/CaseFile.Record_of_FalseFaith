using System;
using Unity.Cinemachine;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerCamera : MonoBehaviour
{
    private CinemachineConfiner2D _cinemachineConfiner2D;
    private GameObject _cameraCollider;
    private CompositeCollider2D _compositeCollider2D;
    private void Start()
    {
        _cinemachineConfiner2D = GetComponent<CinemachineConfiner2D>();
        _cameraCollider = GameObject.FindWithTag("CameraCollider");
        _compositeCollider2D = _cameraCollider.GetComponent<CompositeCollider2D>();
        _cinemachineConfiner2D.BoundingShape2D = _compositeCollider2D;
    }
}
