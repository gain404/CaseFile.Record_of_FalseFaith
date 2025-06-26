using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// 플레이어에게 붙여서 사용하는 상호작용 관련 스크립트입니다.
/// </summary>
public class Interaction : MonoBehaviour
{
    private IInteractable currentInteractable;
    public GameObject curInteractGameObject;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.TryGetComponent(out IInteractable interactable))
        {
            currentInteractable = interactable;
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
}
