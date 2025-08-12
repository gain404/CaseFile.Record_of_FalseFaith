using DG.Tweening;
using UnityEngine;

public class CameraTargetMover : MonoBehaviour
{
    /// <summary>
    /// 카메라 타겟의 위치를 달릴 때 수정해주기 위한 스크립트입니다.
    /// </summary>

    private Vector3 _originalLocalPos;

    private void Awake()
    {
        _originalLocalPos = transform.localPosition;
    }


    public void MoveToOffset(Vector3 offset, float duration)
    {
        transform.DOLocalMove(_originalLocalPos + offset, duration);
    }

    public void ResetPosition(float duration)
    {
        transform.DOLocalMove(_originalLocalPos, duration);
    }
}
