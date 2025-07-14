using System.Collections.Generic;
using UnityEngine;

public class ItemDatabase : MonoBehaviour
{
    //아이템의 이름으로 데이터를 찾아서 아이템 데이터를 넘겨주는 데이터베이스입니다.
    public List<ItemData> allItems;

    public ItemData GetItemByName(string name)
    {
        return allItems.Find(item => item.displayName == name);
    }
}
