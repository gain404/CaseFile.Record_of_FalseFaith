using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// 플레이어에게 붙여서 사용하는 상호작용 관련 스크립트입니다.
/// </summary>
public class Interaction : MonoBehaviour
{
    public IInteractable currentInteractable;
    public GameObject curInteractGameObject;
    public Player player;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.TryGetComponent(out IInteractable interactable))
        {
            currentInteractable = interactable;
            ItemObject currentItem = collision.gameObject.GetComponent<ItemObject>();
            if (currentItem != null)
            {
                player.itemData = currentItem.data;
            }
            //TestCharacterManager.Instance.Player.talkBalloon.SetActive(true);
            //player.talkBalloon.SetActive(true);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        curInteractGameObject = collision.gameObject;
        if (collision.TryGetComponent(out IInteractable interactable))
        {
            if(currentInteractable == interactable)
            {
                currentInteractable = null;
                player.itemData = null;
                //TestCharacterManager.Instance.Player.talkBalloon.SetActive(false);
                //player.talkBalloon.SetActive(false);
            }
        }
    }


    public void OnInteract(InputAction.CallbackContext context)
    {
        if (context.started && currentInteractable != null)
        {
            currentInteractable.OnInteract();
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.I))
        {
            currentInteractable.OnInteract();
        }
    }
}
