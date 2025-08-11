using System;
using UnityEngine;

public class HairProjectileContainer : MonoBehaviour
{
    public void FinishAttack()
    {
        PoolManager.Instance.Return(PoolKey.HairProjectile, gameObject);
    }
}
