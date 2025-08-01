using UnityEngine;

public class BattleSoundZone : MonoBehaviour
{
    private bool hasEntered = false;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (hasEntered) return;

        if (other.CompareTag("Player"))
        {
            StartCoroutine(SoundManager.Instance.FadeBGM(SoundManager.Instance.battleBgm));
            hasEntered = true; // 중복 재생 방지
        }
    }

    // 선택: 전투 구역에서 나올 경우 BGM 복귀
    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            StartCoroutine(SoundManager.Instance.FadeBGM(SoundManager.Instance.defaultBgm));
            hasEntered = false;
        }
    }
}
