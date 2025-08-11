using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.Rendering.Universal;

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
    [SerializeField] private bool isBossRoom;
    [SerializeField] private Light2D globalLight;

    private Player _player;
    private PlayerController _playerController;
    private CinemachineCamera _playerCinemachineCamera;
    private SfxPlayer _sfxPlayer;
    private UIFadePanel _uiFadePanel;
    private UnlockPassageZone _unlockPassageZone;

    private bool _canPassage;
    private bool _isTransitioning;

    private void Start()
    {
        GameObject player = GameObject.FindWithTag("Player");
        GameObject playerCamera = GameObject.FindWithTag("PlayerCamera");

        _player = player.GetComponent<Player>();
        _playerController = _player.GetComponent<PlayerController>();
        _playerCinemachineCamera = playerCamera.GetComponent<CinemachineCamera>();
        _sfxPlayer = GetComponent<SfxPlayer>();
        _unlockPassageZone = GetComponent<UnlockPassageZone>();
        _uiFadePanel = UIManager.Instance.UIFadePanel;

        _canPassage = false;
    }

    private void Update()
    {
        if (_canPassage && !_isTransitioning)
        {
            if (_playerController.playerActions.Interact.WasPerformedThisFrame())
                StartPassage();
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!IsInLayerMask(other.gameObject.layer, hitLayerMask)) return;
        
        _canPassage = (currentPassageZoneType == PassageZoneType.Open) ||
                      (_unlockPassageZone != null && _unlockPassageZone.IsOpen());

        _player.CurrentPassageZone = this;
        
        GuideIconType iconToShow = currentguideIconType;
        if (currentPassageZoneType == PassageZoneType.Close && _unlockPassageZone != null)
            iconToShow = _unlockPassageZone.IsUnlocked ? GuideIconType.OpenDoor : GuideIconType.CloseDoor;

        UIManager.Instance.UIGuideIcon.OnGuideIcon(iconToShow, transform.position);
    }


    private void OnTriggerExit2D(Collider2D other)
    {
        if (!IsInLayerMask(other.gameObject.layer, hitLayerMask)) return;

        _canPassage = false;
        _player.CurrentPassageZone = null;
        UIManager.Instance.UIGuideIcon.OffGuideIcon();
    }

    private bool IsInLayerMask(int layer, LayerMask mask) => ((1 << layer) & mask) != 0;

    public string GetInteractPrompt() => passageInfo.targetPositionName;

    public void OnInteract() { /* 사용 안 함 */ }

    private void StartPassage()
    {
        _isTransitioning = true;

        // 잠긴 문이면 최초 통과 시에만 키 1개 소모 + 영구 해제
        if (currentPassageZoneType == PassageZoneType.Close)
        {
            if (_unlockPassageZone == null ||
                (!_unlockPassageZone.IsUnlocked && !_unlockPassageZone.TryUnlockAndConsume()))
            {
                Debug.Log("문이 잠겨 있습니다. 열쇠가 필요합니다.");
                _isTransitioning = false;
                return;
            }
        }

        if (_sfxPlayer != null)
            _sfxPlayer.PlaySfx(SfxName.DoorOpen);

        _uiFadePanel.AllFade();
        _uiFadePanel.Fade(1.0f, 1.2f, SetPlayerPosition);
    }

    private void SetPlayerPosition()
    {
        _player.transform.position = passageInfo.targetPosition;
        _playerCinemachineCamera.ForceCameraPosition(passageInfo.targetPosition, Quaternion.identity);

        _uiFadePanel.Fade(0f, 1.2f);
        _canPassage = false;
        _isTransitioning = false;

        if (isBossRoom)
            globalLight.color = new Color(0.01f, 0.01f, 0.01f);
    }
}
