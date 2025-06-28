using System.Collections.Generic;
using UnityEngine;

public class PoolManager : MonoBehaviour
{
    public static PoolManager Instance { get; private set; }

    [System.Serializable]
    public class Pool
    {
        public PoolKey key;
        public GameObject prefab;
        public int size;
    }

    [SerializeField] private List<Pool> pools;

    private Dictionary<PoolKey, Queue<GameObject>> _poolDictionary = new Dictionary<PoolKey, Queue<GameObject>>();

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            InitializePools();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void InitializePools()
    {
        foreach (var pool in pools)
        {
            if (pool.prefab == null)
            {
                continue;
            }

            Queue<GameObject> objectPool = new Queue<GameObject>();

            for (int i = 0; i < pool.size; i++)
            {
                GameObject obj = Instantiate(pool.prefab);
                obj.SetActive(false);
                objectPool.Enqueue(obj);
            }

            if (!_poolDictionary.ContainsKey(pool.key))
            {
                _poolDictionary.Add(pool.key, objectPool);
            }
            else
            {
                Debug.LogWarning($"[PoolManager] Duplicate pool key detected: {pool.key}. Skipping this pool.");
            }
        }

        Debug.Log("[PoolManager] Initialization complete.");
    }

    /// <summary>
    /// 풀에서 오브젝트를 가져온 후 위치/회전을 설정하여 반환
    /// </summary>
    public GameObject Get(PoolKey key, Vector3 position, Quaternion rotation)
    {
        if (!_poolDictionary.ContainsKey(key))
        {
            Debug.LogWarning($"[PoolManager] Pool with key '{key}' does not exist.");
            return null;
        }

        Queue<GameObject> objectPool = _poolDictionary[key];
        GameObject obj;

        if (objectPool.Count > 0)
        {
            obj = objectPool.Dequeue();
        }
        else
        {
            // 풀 부족 시 자동 확장
            obj = Instantiate(pools.Find(p => p.key == key).prefab);
            Debug.LogWarning($"[PoolManager] Pool '{key}' exhausted. Instantiating additional object.");
        }

        obj.transform.position = position;
        obj.transform.rotation = rotation;
        obj.SetActive(true);

        return obj;
    }

    /// <summary>
    /// 사용한 오브젝트를 풀로 되돌린다
    /// </summary>
    public void Return(PoolKey key, GameObject obj)
    {
        if (!_poolDictionary.ContainsKey(key))
        {
            Debug.LogWarning($"[PoolManager] Pool with key '{key}' does not exist.");
            Destroy(obj);
            return;
        }

        obj.SetActive(false);
        _poolDictionary[key].Enqueue(obj);
    }
}