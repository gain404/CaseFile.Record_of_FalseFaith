using System;
using Unity.VisualScripting;
using UnityEngine;

// 인터랙션 가능한 객체에 상속할 인터페이스
public interface IInteractable
{
    public string GetInteractPrompt();  // UI에 표시할 정보
    public void OnInteract();           // 인터랙션 호출
}

/// <summary>
/// 어떤 아이템 하나에 대한 정보가 담겨있는 스크립트입니다.
/// </summary>
public class ItemObject : MonoBehaviour,IInteractable
{
    private Player _player;
    public ItemData data;

    private void Awake()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        _player = player.GetComponent<Player>();
    }

    private void Start()
    {
        
    }

    public string GetInteractPrompt()//인벤토리 등에서 아이템 이름과 설명을 보여줄 수 있음
    {
        string str = $"{data.itemName}\n{data.itemDescription}";
        return str;
}

    
    public void OnInteract()//플레이어가 아이템을 주웠을 때 어떻게 될 지
    {
        _player.itemData = data;
        _player.addItem?.Invoke();
        InventoryManager.Instance.AddItem(data.idx, 1);
        Debug.Log($"{data.idx} 아이템 먹었으니 퀘스트 확인");
        ObjectiveManager.Instance.OnItemCollected($"{data.idx}", 1);

        Destroy(gameObject);
    }
}
