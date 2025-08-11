using System;
using System.Collections.Generic;

[Serializable]
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

    // 열린 문 ID들
    public List<string> unlockedPassages = new();

    // ✅ 생성자에서는 "순수 기본값"만 설정 (Unity 접근 금지)
    public SaveData()
    {
        playerName = "Player";
        posX = posY = posZ = 0f;
        health = 10;
        sceneName = "MainScene";
        saveTime = DateTime.Now;

        // 여기서 Unity/싱글턴 접근 금지!
        inventoryItems = new List<InventoryItem>();
        activeObjectives = new List<ObjectiveData>();
        completedObjectives = new List<ObjectiveData>();
    }
}