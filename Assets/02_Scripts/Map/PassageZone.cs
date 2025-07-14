using System;
using UnityEngine;

public class PassageZone : MonoBehaviour
{
    [SerializeField] private PassageInfo passageInfo;
    [SerializeField] private LayerMask hitLayerMask;
    private PassageManager _passageManager;
    private SfxPlayer _sfxPlayer;

    private void Start()
    {
        _passageManager = PassageManager.Instance;
        _sfxPlayer = GetComponent<SfxPlayer>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (((1 << other.gameObject.layer) & hitLayerMask) != 0)
        {
            _passageManager.SetInfo(passageInfo.isSceneChange, passageInfo.sceneName, passageInfo.targetPosition,
                passageInfo.targetPositionName, _sfxPlayer);
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
