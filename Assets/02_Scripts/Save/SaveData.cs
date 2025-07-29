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

    // 퀘스트 데이터
    public List<ObjectiveData> activeObjectives;    // 진행 중인 목표
    public List<ObjectiveData> completedObjectives; // 완료된 목표

    // 기본 생성자
    public SaveData()
    {
        playerName = "Player";
        posX = posY = posZ = 0f;
        health = 10;
        sceneName = "MainScene";
        saveTime = DateTime.Now;
        inventoryItems = InventoryManager.Instance.GetInventoryData();

        // 퀘스트 진행도 저장
        activeObjectives = ObjectiveManager.Instance?.GetActiveObjectives();
        completedObjectives = ObjectiveManager.Instance != null
            ? new List<ObjectiveData>(ObjectiveManager.Instance.GetCompletedObjectives())
            : new List<ObjectiveData>();
    }
}
