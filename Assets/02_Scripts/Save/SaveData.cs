using System;
using System.Collections.Generic;

[System.Serializable]
public class SaveData
{

    public string playerName;
    public float posX, posY, posZ;
    public int health;
    public string sceneName;
    public DateTime saveTime;


    // 인벤토리 데이터
    public List<InventoryItem> inventoryItems;

    // 기본 생성자
    public SaveData()
    {
        playerName = "Player";
        posX = posY = posZ = 0f;
        health = 10;
        sceneName = "MainScene";
        saveTime = DateTime.Now;
        inventoryItems = InventoryManager.Instance.GetInventoryData();
    }
}
