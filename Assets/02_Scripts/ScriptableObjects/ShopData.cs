using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "New ShopData", menuName = "ScriptableObjects/Shop Data")]
public class ShopData : ScriptableObject
{
    public List<ItemData> itemsForSale;
}