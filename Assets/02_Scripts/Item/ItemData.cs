using UnityEngine;

public enum ItemType
{
    Recover,//회복 아이템
    Clue, //단서 아이템
    Key, //열쇠
    StoryFlag, //스토리 진행시 필요
}

/// <summary>
/// 아이템의 정보를 담고 있는 스크립트입니다.
/// </summary>
[CreateAssetMenu(fileName = "Item", menuName = "ScriptableObjects/New Item")]
public class ItemData : ScriptableObject
{
    public int idx;
    public string itemName;
    public string itemDescription;
    public Sprite itemSprite;
    public ItemType itemType;
    public int healHP;
    public int healStamina;
    public bool canSearch;
    public int stageAvailable;
    public string acquireCondition;
    public int itemPrice;
    public int maxStackAmount;

    public ItemData(int idx, string itemName, string itemDescription, Sprite itemSprite,
        ItemType itemType, int healHP, int healStamina, bool canSearch, int stageAvailable, string acquireCondition,
        int itemPrice, int maxStackAmount)
    {
        this.idx = idx;
        this.itemName = itemName;
        this.itemDescription = itemDescription;
        this.itemSprite = itemSprite;
        this.itemType = itemType;
        this.healHP = healHP;
        this.healStamina = healStamina;
        this.canSearch = canSearch;
        this.stageAvailable = stageAvailable;
        this.acquireCondition = acquireCondition;
        this.itemPrice = itemPrice;
        this.maxStackAmount = maxStackAmount;
    }

    //[Header("Info")]
    //public string displayName;
    //public string description;
    //public ItemType type;
    //public int price;
    //public Sprite icon;
    //public GameObject dropPrefab;

    //[Header("Stacking")]
    //public bool canStack;
    //public int maxStackAmount;

    [Header("Effect")]
    public ItemEffectSO Effect;
}
