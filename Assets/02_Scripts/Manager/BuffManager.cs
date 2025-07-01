using UnityEngine;
using System.Collections.Generic;

public class BuffManager : MonoBehaviour
{
    private readonly List<Buff> activeBuffs = new List<Buff>();
    private StatManager ownerStats;

    private void Awake()
    {
        ownerStats = GetComponent<StatManager>();
    }

    private void Update()
    {
        for (int i = activeBuffs.Count - 1; i >= 0; i--)
        {
            Buff buff = activeBuffs[i];
            buff.timeRemaining -= Time.deltaTime;
            
            if (buff is DamageBuff dotBuff)
            {
                dotBuff.OnTick(ownerStats);
            }

            if (buff.timeRemaining <= 0)
            {
                buff.RemoveEffect(ownerStats);
                activeBuffs.RemoveAt(i);
            }
        }
    }

    public void ApplyBuff(Buff buff)
    {
        Buff newBuff = buff.Clone();
        newBuff.timeRemaining = newBuff.Duration;
        newBuff.ApplyEffect(ownerStats);
        activeBuffs.Add(newBuff);
    }
}