using UnityEngine;

public class WeaponHandler : MonoBehaviour
{
    public Vector2 boxCastSize = new Vector2(0.5f, 0.5f);
    public RaycastHit2D hit;
    [SerializeField] private LayerMask layerMask;
    [SerializeField] private WeaponData[] weaponData;
    private EnemyHealth _enemyHealth;
    private Player _player;
    private TalismanProjectile _talismanProjectile;

    private void Awake()
    {
        _player = GetComponent<Player>();
        _talismanProjectile = GetComponentInChildren<TalismanProjectile>();

        _talismanProjectile.damage = weaponData[1].damage;
    }
    
    public void SwordAttack()
    {
        Vector2 halfSize = boxCastSize * 0.5f;
        hit = Physics2D.BoxCast(transform.position + (Vector3)_player.PlayerController.lookDirection * halfSize.x
            ,boxCastSize, 0f, Vector2.zero, 0f, layerMask);
        
        if (hit.collider != null)
        {
            _enemyHealth.TakeDamage(weaponData[0].damage);
        }
        
    }
    
#if UNITY_EDITOR
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(transform.position, boxCastSize);
    }
#endif
    
    public void TalismanAttack()
    {
        //위치 및 각도 조정하기
        PoolManager.Instance.Get(PoolKey.PlayerAmuletProjectile, transform.position, Quaternion.identity);
    }
    
}
