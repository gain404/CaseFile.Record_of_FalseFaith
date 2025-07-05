using UnityEngine;

public enum ItemType
{
    Resource,//보스 같은 거 잡으면 자동으로 얻어지는 아이템 같은 거
    Consumable, //물약, 부적 충전용 아이템 같은 거
    Special//버릴 수 없는 중요 아이템 같은 거
}

/// <summary>
/// 아이템의 정보를 담고 있는 스크립트입니다.
/// </summary>
[CreateAssetMenu(fileName = "Item", menuName = "ScriptableObjects/New Item")]
public class ItemData : ScriptableObject
{
    [Header("Info")]
    public string displayName;
    public string description;
    public ItemType type;
    public Sprite icon;
    public GameObject dropPrefab;

    [Header("Stacking")]
    public bool canStack;
    public int maxStackAmount;

    [Header("Effect")]
    public ItemEffectSO Effect;
}
