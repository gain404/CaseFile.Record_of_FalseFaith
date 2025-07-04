using UnityEngine;

public enum WeaponType
{
    Sword,
    Gun,
}

[CreateAssetMenu(fileName = "WeaponData", menuName = "ScriptableObjects/WeaponData")]
public class WeaponData : ScriptableObject
{
    public WeaponType weaponType;
    public int damage;
    public float damageRate;
    public float damageDistance;
}
