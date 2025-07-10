using System;
using UnityEngine;

public class PassageZone : MonoBehaviour
{
    [SerializeField] private PassageInfo passageInfo;
    [SerializeField] private LayerMask hitLayerMask;
    private PassageManager _passageManager;

    private void Start()
    {
        _passageManager = PassageManager.Instance;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (((1 << other.gameObject.layer) & hitLayerMask) != 0)
        {
            Debug.Log(_passageManager == null ? "_passageManager is null" : "_passageManager is NOT null");
            _passageManager.SetInfo(passageInfo.isSceneChange, passageInfo.sceneName, passageInfo.targetPosition);
            _passageManager.canMovement = true;
        }
    }
    
    private void OnTriggerExit2D(Collider2D other)
    {
        if (((1 << other.gameObject.layer) & hitLayerMask) != 0)
        {
            _passageManager.canMovement = false;
        }
    }
}
