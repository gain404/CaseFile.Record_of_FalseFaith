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

    //public List<string> savedItemIDs = new List<string>();
    //// ItemData는 SO라서 저장이 불가능하니까 따로 저장하기 위해서 만든 스크립트입니다. scv로 대체

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
