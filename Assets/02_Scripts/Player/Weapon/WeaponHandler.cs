using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponHandler : MonoBehaviour
{
    public WeaponType CurrentWeapon { get; private set; }
    public Vector2 boxCastSize = new Vector2(0.5f, 0.5f);
    public int weaponCount;
    [SerializeField] private LayerMask layerMask;
    [SerializeField] private WeaponData[] weaponDataArray;
    [SerializeField] private Transform projectileTransform;
    
    private Dictionary<WeaponType, WeaponData> _weaponData;
    private RaycastHit2D _hit;
    private EnemyHealth _enemyHealth;
    private Player _player;
    private BulletProjectile _bulletProjectile;
    private SwordAttack _swordAttackPoint;
    
    private void Awake()
    {
        _player = GetComponent<Player>();
        //_bulletProjectile = GetComponentInChildren<BulletProjectile>();
        _swordAttackPoint = GetComponentInChildren<SwordAttack>();
        
        _weaponData = new Dictionary<WeaponType, WeaponData>(weaponDataArray.Length);
        foreach (var wd in weaponDataArray)
        {
            _weaponData.Add(wd.weaponType, wd);
        }
    }

    private void Update()
    {
        EquipWeapon();
    }
    
    private void EquipWeapon()
    {
        if (_player.PlayerController.playerActions.Weaponchange.triggered)
        {
            if (weaponCount == 0)
            {
                CurrentWeapon = WeaponType.Sword;
                weaponCount++;
            }
            else if (weaponCount > 0)
            {
                CurrentWeapon = WeaponType.Gun;
                weaponCount = 0;
            }
        }
    }
    
    public void SwordAttack()
    {
        _swordAttackPoint.gameObject.SetActive(true);
    }

    public void StopSwordAttack()
    {
        _swordAttackPoint.gameObject.SetActive(false);
    }
    
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (!collision.collider.TryGetComponent<SwordAttack>(out _))
            return;

        if (collision.gameObject.TryGetComponent<IDamagable>(out var target))
        {
            target.TakeDamage(_weaponData[WeaponType.Sword].damage);
        }
    }
    
    public void GunAttack()
    {
        GameObject bullet = PoolManager.Instance.Spawn(PoolKey.PlayerAmuletProjectile,
            projectileTransform.position, projectileTransform.rotation);
        
        _bulletProjectile = bullet.GetComponent<BulletProjectile>();
        
        _bulletProjectile.damage = _weaponData[WeaponType.Gun].damage;
        _bulletProjectile.damageDistance = _weaponData[WeaponType.Gun].damageDistance;
        
        float xSign = _player.transform.localScale.x < 0f ? -1f : 1f;
        Vector2 direction = new Vector2(xSign, 0).normalized;
        float damageRate = _weaponData[WeaponType.Gun].bulletSpeed;
        
        _bulletProjectile.ThrowBullet(direction, damageRate);
    }
}
