using System.Collections.Generic;
using UnityEngine;

public class InventoryManager : MonoBehaviour
{
    /// <summary>
    /// 다른 씬으로 이동해도 인벤토리를 유지해주는 싱글톤입니다.
    /// </summary>

    public static InventoryManager Instance;

    public List<ItemData> items = new List<ItemData>();


    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    public void AddItem(ItemData item)
    {
        items.Add(item);
    }

    public void RemoveItem(ItemData item)
    {
        items.Remove(item);
    }
}
