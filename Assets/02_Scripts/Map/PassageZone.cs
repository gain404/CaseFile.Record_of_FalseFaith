using System;
using UnityEngine;

public class PassageZone : MonoBehaviour, IInteractable
{
    [SerializeField] private PassageInfo passageInfo;
    [SerializeField] private LayerMask hitLayerMask;
    private PassageManager _passageManager;
    private SfxPlayer _sfxPlayer;
    private Player _player;

    private void Start()
    {
        _passageManager = PassageManager.Instance;
        _sfxPlayer = GetComponent<SfxPlayer>();
        GameObject player = GameObject.FindWithTag("Player");
        _player = player.GetComponent<Player>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (((1 << other.gameObject.layer) & hitLayerMask) != 0)
        {
            _passageManager.SetInfo(passageInfo.isSceneChange, passageInfo.sceneName, passageInfo.targetPosition,
                passageInfo.targetPositionName, _sfxPlayer);
            _passageManager.canMovement = true;
            _player.CurrentPassageZone = this;
        }
    }
    
    private void OnTriggerExit2D(Collider2D other)
    {
        if (((1 << other.gameObject.layer) & hitLayerMask) != 0)
        {
            _passageManager.canMovement = false;
        }
    }

    public string GetInteractPrompt()
    {
        return passageInfo.targetPositionName;
    }

    public void OnInteract()
    {
        //
    }
}
