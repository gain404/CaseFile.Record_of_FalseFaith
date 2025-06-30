using UnityEngine;

public class TestUIManager : MonoBehaviour
{
    public static TestUIManager Instance;

    public UIInventory uiInventory;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }
}
