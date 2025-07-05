using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class PoolManager : MonoBehaviour
{
    public static PoolManager Instance { get; private set; }

    [System.Serializable]
    public class Pool
    {
        //key는 같이 있는 PoolKey에 등록해서 사용
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
            
            //PoolManager자식에 각각 projectile을 담을 컨테이너를 만들고 거기에 projectile을 구분해서 저장
            Transform container = new GameObject($"{pool.key}_Container").transform;
            container.SetParent(this.transform);

            for (int i = 0; i < pool.size; i++)
            {
                GameObject obj = Instantiate(pool.prefab, container);
                obj.SetActive(false);
                objectPool.Enqueue(obj);
            }

            _poolDictionary.TryAdd(pool.key, objectPool);//중복 방지
        }
    }
    
    //Projectile 발사
    public void Get(PoolKey key, Vector3 position, Quaternion rotation)
    {
        if (!_poolDictionary.ContainsKey(key))
        {
            return;
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
        }
        obj.transform.position = position;
        obj.transform.rotation = rotation;
        obj.SetActive(true);
    }
    
    //Projectile 소멸
    public void Return(PoolKey key, GameObject obj)
    {
        if (!_poolDictionary.ContainsKey(key))
        {
            Destroy(obj);
            return;
        }

        obj.SetActive(false);
        _poolDictionary[key].Enqueue(obj);
    }
    
    public GameObject Spawn(PoolKey key, Vector3 position, Quaternion rotation)
    {
        if (!_poolDictionary.ContainsKey(key))
        {
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
        }
        obj.transform.position = position;
        obj.transform.rotation = rotation;
        obj.SetActive(true);
        return obj;
    }
}