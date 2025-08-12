using System;
using UnityEngine;

public enum MoneyType
{
    Coin = 500,
    Money = 1000
}

public class MoneyObject : MonoBehaviour, IInteractable
{
    [SerializeField] private MoneyType type;
    [SerializeField] private LayerMask layerMask;
    private Player _player;
    private bool _isTriggered;

    private void Awake()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        _player = player.GetComponent<Player>();
    }
    
    private void Update()
    {
        if (_isTriggered && _player.PlayerController.playerActions.Interact.WasPerformedThisFrame())
        {
            GetMoney();
        }
    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        if ((1 << other.gameObject.layer & layerMask.value) != 0)
        {
            _isTriggered = true;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if ((1 << other.gameObject.layer & layerMask.value) != 0)
        {
            _isTriggered = false;
        }
    }
    
    public string GetInteractPrompt()
    {
        return null;
    }

    public void OnInteract()
    {
        //
    }

    private void GetMoney()
    {
        float amount = (int)type;
        _player.PlayerStat.AddStat(StatType.Money, amount);
        
        Destroy(gameObject);
    }
}
