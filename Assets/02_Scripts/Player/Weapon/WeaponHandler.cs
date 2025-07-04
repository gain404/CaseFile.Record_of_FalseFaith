using System.Collections.Generic;
using UnityEngine;

public class WeaponHandler : MonoBehaviour
{
    public WeaponType CurrentWeapon { get; private set; }
    public Vector2 boxCastSize = new Vector2(0.5f, 0.5f);
    [SerializeField] private LayerMask layerMask;
    
    private Dictionary<WeaponType, WeaponData> weaponData;
    private RaycastHit2D _hit;
    private EnemyHealth _enemyHealth;
    private Player _player;
    private BulletProjectile _bulletProjectile;
    private int _weaponCount;
    
    private void Awake()
    {
        _player = GetComponent<Player>();
        _bulletProjectile = GetComponentInChildren<BulletProjectile>();
    }

    private void Update()
    {
        EquipWeapon();
    }
    
    private void EquipWeapon()
    {
        if (_player.PlayerController.playerActions.Weaponchange.ReadValue<float>() >= 0.5f)
        {
            if (_weaponCount == 0)
            {
                CurrentWeapon = WeaponType.Sword;
                _weaponCount++;
            }
            else if (_weaponCount > 0)
            {
                CurrentWeapon = WeaponType.Gun;
                _weaponCount = 0;
            }
        }
    }
    
    public void SwordAttack()
    {
        Vector2 halfSize = boxCastSize * 0.5f;
        _hit = Physics2D.BoxCast(transform.position + (Vector3)_player.PlayerController.lookDirection * halfSize.x
            ,boxCastSize, 0f, Vector2.zero, 0f, layerMask);
        
        if (_hit.collider != null)
        {
            _enemyHealth.TakeDamage(weaponData[WeaponType.Sword].damage);
        }
        
    }
    
#if UNITY_EDITOR
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        
        Vector2 halfSize = boxCastSize * 0.5f;
        Vector3 center = transform.position + (Vector3)_player.PlayerController.lookDirection * halfSize.x;
        
        Gizmos.DrawWireCube(center, boxCastSize);
    }
#endif
    
    public void GunAttack()
    {
        _bulletProjectile.damage = weaponData[WeaponType.Gun].damage;
        _bulletProjectile.damageDistance = weaponData[WeaponType.Gun].damageDistance;
        
        //위치 및 각도 조정하기
        Vector2 talismanPosition = transform.position;
        Quaternion talismanRotation = transform.rotation;
        
        PoolManager.Instance.Get(PoolKey.PlayerAmuletProjectile, talismanPosition, talismanRotation);
        Shoot();
    }

    public void StopGunAttack()
    {
        PoolManager.Instance.Return(PoolKey.PlayerAmuletProjectile, _bulletProjectile.gameObject);
    }

    private void Shoot()
    {
        float xSign = _player.PlayerSpriteRenderer.flipX ? -1f : 1f;
        Vector2 direction = new Vector2(xSign, 1).normalized;
        float damageRate = weaponData[WeaponType.Gun].damageRate;
        
        _bulletProjectile.ThrowTalismanProjectile(direction, damageRate);
    }
    
}
