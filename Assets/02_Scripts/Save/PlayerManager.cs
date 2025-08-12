using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    public static PlayerManager Instance;
    public GameObject playerObject;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);

            // 플레이어 오브젝트 자동 찾기 (직접 넣어줘도 됨)
            if (playerObject == null)
            {
                playerObject = GameObject.FindGameObjectWithTag("Player");
            }
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // 플레이어 오브젝트 가져오기
    public GameObject GetPlayer()
    {
        if (playerObject == null)
        {
            playerObject = GameObject.FindGameObjectWithTag("Player");
        }
        return playerObject;
    }

    // 플레이어 위치 가져오기
    public Vector3 GetPlayerPosition()
    {
        GameObject player = GetPlayer();
        return player != null ? player.transform.position : Vector3.zero;
    }
}
