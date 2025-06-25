using TMPro;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// 플레이어에게 붙여서 사용하는 상호작용 관련 스크립트입니다.
/// </summary>
public class Interaction : MonoBehaviour
{
    public LayerMask layerMask;
    public Collider2D collider2d;

    public GameObject curInteractGameObject;  // 현재 상호작용 게임오브젝트
    private IInteractable curInteractable;    // 현재 상호작용 인터페이스

    public TextMeshProUGUI promptText;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {


        
    }

    private void SetPromptText()
    {
        promptText.gameObject.SetActive(true);
        promptText.text = curInteractable.GetInteractPrompt();
    }

    public void OnInteractInput(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Started && curInteractable != null)
        {
            curInteractable.OnInteract();
            curInteractGameObject = null;
            curInteractable = null;
            promptText.gameObject.SetActive(false);
        }
    }

    void OnCollisionEnter2D(Collision2D collision)//플레이어의 콜라이더와 부딪힌 콜라이더 체크
    {
        if (collision.gameObject.CompareTag("Item"))//아이템과 닿았다면 내용 출력
        {
            curInteractGameObject = collision.gameObject;//지금 닿아있는 오브젝트를 보여줌
            curInteractable = collision.gameObject.GetComponent<IInteractable>();//그 오브젝트에 IInteractable를 가져옴
            SetPromptText();
        }
        else
        {
            curInteractGameObject = null ;
            curInteractable = null;
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        curInteractGameObject = null;
        curInteractable = null;
    }
}
