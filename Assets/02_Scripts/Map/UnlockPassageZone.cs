using UnityEngine;

public enum UnLockkeyType
{
    Gukbabkey = 1301,
    ShamanKey = 1302,
    BossStageKey = 1303,
}

public class UnlockPassageZone : MonoBehaviour
{
    [SerializeField] private UnLockkeyType unLockkeyType;
    [SerializeField] private string passageId; // 🔑 문 고유 ID(양방향 문이면 같은 값)
    
    private InventoryManager _inventoryManager;
    private bool _unlocked;

    private void Awake()
    {
        _inventoryManager = InventoryManager.Instance;

        // ✅ 로드된 세이브 데이터로 상태 복원
        var data = SaveManager.Instance?.ActiveData;
        if (data != null && !string.IsNullOrEmpty(passageId) && data.unlockedPassages.Contains(passageId))
            _unlocked = true;
    }

    public bool IsUnlocked => _unlocked;

    public bool IsOpen()
    {
        if (_unlocked) return true;
        if (_inventoryManager == null || _inventoryManager.inventory == null) return false;

        return _inventoryManager.inventory.Exists(it =>
            it.itemId == (int)unLockkeyType && it.quantity > 0);
    }

    /// <summary>
    /// 아직 해제 전이면: 키 1개 소모 + 영구 해제 + 세이브 목록에 ID 등록.
    /// 이미 해제면 true.
    /// </summary>
    public bool TryUnlockAndConsume()
    {
        if (_unlocked) return true;

        var inv = _inventoryManager?.inventory;
        if (inv == null) return false;

        int keyId = (int)unLockkeyType;

        for (int i = 0; i < inv.Count; i++)
        {
            if (inv[i].itemId == keyId && inv[i].quantity > 0)
            {
                // 1) 키 1개 소모
                inv[i].quantity--;
                if (inv[i].quantity <= 0) inv.RemoveAt(i);

                // 2) 인벤토리 UI 즉시 갱신
                UIManager.Instance?.UIInventory?.RefreshUI();

                // 3) 영구 해제
                _unlocked = true;

                // 4) 세이브 데이터에 문 ID 등록
                var data = SaveManager.Instance?.ActiveData;
                if (data != null && !string.IsNullOrEmpty(passageId) &&
                    !data.unlockedPassages.Contains(passageId))
                {
                    data.unlockedPassages.Add(passageId);
                }
                return true;
            }
        }
        return false;
    }

    
}
