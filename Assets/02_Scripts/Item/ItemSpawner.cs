using UnityEngine;

public class ItemSpawner : MonoBehaviour
{
    /// <summary>
    /// 아이템을 지정한 위치에 소환시키는 스크립트입니다.
    /// 어떤 이벤트 진행 후 특정 위치에 아이템을 스폰시키는 등의 용도로 사용할 수 있습니다.
    /// 혹시 몰라서 만들어놓음...
    /// </summary>

    [Header("스포너 설정")]
    public GameObject itemPickupPrefab;
    public int itemIdToSpawn = 1;
    public int quantityToSpawn = 1;

    [Header("스폰 위치")]
    public Transform[] spawnPoints;
    public bool spawnOnStart = true;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (spawnOnStart)
        {
            SpawnItems();
        }
    }

    [ContextMenu("Spawn Items")]
    public void SpawnItems()
    {
        if (itemPickupPrefab == null)
        {
            Debug.LogError("ItemPickup 프리팹이 설정되지 않았습니다!");
            return;
        }

        if (spawnPoints == null || spawnPoints.Length == 0)
        {
            Debug.LogError("스폰 포인트가 설정되지 않았습니다!");
            return;
        }

        foreach (Transform spawnPoint in spawnPoints)
        {
            if (spawnPoint != null)
            {
                GameObject itemObject = Instantiate(itemPickupPrefab, spawnPoint.position, spawnPoint.rotation);
                ItemPickup itemPickup = itemObject.GetComponent<ItemPickup>();

                if (itemPickup != null)
                {
                    itemPickup.itemId = itemIdToSpawn;
                    itemPickup.quantity = quantityToSpawn;
                }
            }
        }

        Debug.Log($"아이템 스폰 완료: {spawnPoints.Length}개 위치에 ID {itemIdToSpawn} 아이템 생성");
    }

    // 특정 위치에 아이템 스폰
    public void SpawnItemAt(Vector2 position, int itemId, int quantity = 1)
    {
        if (itemPickupPrefab == null) return;

        GameObject itemObject = Instantiate(itemPickupPrefab, position, Quaternion.identity);
        ItemPickup itemPickup = itemObject.GetComponent<ItemPickup>();

        if (itemPickup != null)
        {
            itemPickup.itemId = itemId;
            itemPickup.quantity = quantity;
        }
    }
}
