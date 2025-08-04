using System;
using System.Collections.Generic;
using UnityEngine;

public enum UnLockkeyType
{
    Gukbabkey = 1301,
    ShamanKey = 1302
}

public class UnlockPassageZone : MonoBehaviour
{
    [SerializeField] private UnLockkeyType unLockkeyType;
    private InventoryManager _inventoryManager;

    private void Awake()
    {
        _inventoryManager = InventoryManager.Instance;
    }
    
    public bool IsOpen()
    {
        if (_inventoryManager == null || _inventoryManager.inventory == null) return false;
        
        InventoryItem item = _inventoryManager.inventory.Find(item => item.itemId == (int)unLockkeyType && item.quantity > 0);
        return item != null;
    }
}