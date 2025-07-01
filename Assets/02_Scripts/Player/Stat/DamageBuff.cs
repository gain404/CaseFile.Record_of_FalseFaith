using UnityEngine;

// Buff 클래스를 상속받아 새로운 버프 타입을 정의합니다.
[System.Serializable]
public class DamageBuff : Buff
{
    private float tickTimer;

    // 이 버프는 스탯을 직접 바꾸지 않으므로 Apply/RemoveEffect는 비워둡니다.
    public override void ApplyEffect(StatManager targetStats) { }
    public override void RemoveEffect(StatManager targetStats) { }

    // Update에서 매 프레임 호출될 함수
    public void OnTick(StatManager targetStats)
    {
        tickTimer -= Time.deltaTime;
        if (tickTimer <= 0f)
        {
            Debug.Log("독 데미지! " + Value);
            targetStats.TakeDamage(Value); // Value 만큼의 데미지를 줌
            tickTimer = 1f; // 1초마다 반복
        }
    }
}