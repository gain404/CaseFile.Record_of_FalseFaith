using UnityEngine;
using TMPro;

public class UIPrompt : MonoBehaviour
{
    [SerializeField] private GameObject interactPanel;
    [SerializeField] private TMP_Text promptText;
    
    private IInteractable _currentTarget;
    private Player _player;

    private void Start()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        _player = player.GetComponent<Player>();
        interactPanel.SetActive(false);
    }

    private void Update()
    {
        if (UIManager.Instance.UIInventory != null && UIManager.Instance.UIInventory.IsOpen())
        {
            if (interactPanel.activeSelf)
            {
                interactPanel.SetActive(false);
                _currentTarget = null;
            }
            return;
        }
        
        if (UIManager.Instance.UIDialogue.CurrentState != DialogueState.Inactive)
        {
            if (interactPanel.activeSelf)
            {
                interactPanel.SetActive(false);
                _currentTarget = null;
            }
            return;
        }
        
        if (interactPanel.activeSelf && _player.PlayerController.playerActions.Interact.WasPressedThisFrame())
        {
            interactPanel.SetActive(false);
            _currentTarget = null; 
            return;
        }

        IInteractable newTarget = null;
        if (_player.CurrentInteractableNPC != null)
        {
            newTarget = _player.CurrentInteractableNPC;
        }
        else if (_player.CurrentInteractableItem != null)
        {
            newTarget = _player.CurrentInteractableItem;
        }
        else if (_player.itemData != null)
        {
            newTarget = new ItemDataInteractable(_player.itemData);
        }
        else if(_player.CurrentPassageZone != null)
        {
            newTarget = _player.CurrentPassageZone;
        }
        
        if (newTarget != _currentTarget)
        {
            _currentTarget = newTarget;
            interactPanel.SetActive(_currentTarget != null);

            if (_currentTarget != null)
            {
                // 타겟의 종류에 맞는 프롬프트 텍스트를 가져와 UI에 표시
                promptText.text = _currentTarget.GetInteractPrompt();
            }
        }
    }
}