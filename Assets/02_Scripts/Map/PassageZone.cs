using Unity.Cinemachine;
using UnityEngine;

public enum PassageZoneType
{
    Open,
    Close
}

public class PassageZone : MonoBehaviour, IInteractable
{
    [SerializeField] private PassageInfo passageInfo;
    [SerializeField] private LayerMask hitLayerMask;
    [SerializeField] private GuideIconType currentguideIconType;
    [SerializeField] private PassageZoneType currentPassageZoneType;
    private Player _player;
    private PlayerController _playerController;
    private CinemachineCamera _playerCinemachineCamera;
    private SfxPlayer _sfxPlayer;
    private FadeManager _fadeManager;
    private UnlockPassageZone _unlockPassageZone;
    private bool _canPassage;
    

    private void Start()
    {
        GameObject player = GameObject.FindWithTag("Player");
        GameObject playerCamera = GameObject.FindWithTag("PlayerCamera");
        _player = player.GetComponent<Player>();
        _playerController = _player.GetComponent<PlayerController>();
        _playerCinemachineCamera = playerCamera.GetComponent<CinemachineCamera>();
        _sfxPlayer = GetComponent<SfxPlayer>();
        _fadeManager = FadeManager.Instance;
        _unlockPassageZone = GetComponent<UnlockPassageZone>();
        _canPassage = false;
    }
    
    private void Update()
    {
        if (_canPassage)
        {
            if (_playerController.playerActions.Interact.WasPerformedThisFrame())
            {
                StartPassage();
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!IsInLayerMask(other.gameObject.layer, hitLayerMask))
            return;

        if (currentPassageZoneType == PassageZoneType.Open || _unlockPassageZone.IsOpen())
        {
            _canPassage = true;
        }
        else
        {
            _canPassage = false;
        }
        _player.CurrentPassageZone = this;
        UIManager.Instance.UIGuideIcon.OnGuideIcon(currentguideIconType, transform.position);
    }
    
    private void OnTriggerExit2D(Collider2D other)
    {
        if (IsInLayerMask(other.gameObject.layer, hitLayerMask))
        {
            _canPassage = false;
            _player.CurrentPassageZone = null;
            UIManager.Instance.UIGuideIcon.OffGuideIcon();
        }
    }

    private bool IsInLayerMask(int layer, LayerMask mask)
    {
        return ((1 << layer) & mask) != 0;
    }
    
    public string GetInteractPrompt()
    {
        return passageInfo.targetPositionName;
    }

    public void OnInteract()
    {
        //
    }
    
    private void StartPassage()
    {
        if (_sfxPlayer != null)
        {
            _sfxPlayer.PlaySfx(SfxName.DoorOpen);
        }
        _fadeManager.OnCanvas();
        _fadeManager.OrderChange(200);
        _fadeManager.Fade(1.0f, 1.2f, SetPlayerPosition);
    }
    
    private void SetPlayerPosition()
    {
        Quaternion quaternion = new Quaternion(0, 0, 0, 0);
        _player.transform.position = passageInfo.targetPosition;
        _playerCinemachineCamera.ForceCameraPosition(passageInfo.targetPosition, quaternion);
        _fadeManager.Fade(0, 1.2f, _fadeManager.OffCanvas);
        _canPassage = false;
    }
}
