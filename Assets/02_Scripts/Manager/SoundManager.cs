using System.Collections;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    //아이템 줍는 소리, 문 소리 등 
    public static SoundManager Instance;

    [Header("Audio Sources")]
    public AudioSource bgmSource;
    public AudioSource sfxSource;
    public AudioSource uiSource;


    [Header("BGM Clips")]
    public AudioClip defaultBgm;
    public AudioClip battleBgm;
    [Header("Audio Clips")]
    public AudioClip itemPickupClip; // 아이템 주웠을 때
    public AudioClip buttonClickClip; // 버튼 눌렀을 때
    public AudioClip doorOpen; // 문 상호작용할 때
    public AudioClip objectiveGet; // 목표 획득 시
    public AudioClip fileOpen; // 파일 열 때
    public AudioClip inventoryOpen; // 인벤토리 열 때
    public AudioClip mapOpen; // 맵 열 때

    [Header("Attack SFX")]
    public AudioClip[] swordAttackClips; // 칼 공격 효과음들을 배열로 관리

    private void Awake()
    {
        // 싱글톤 패턴
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // 씬 전환 시 유지
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    private void Start()
    {
        if (defaultBgm != null)
        {
            PlayBGM(defaultBgm); // 배경음악 있으면 재생 시켜주기
        }
    }

    // BGM 재생
    public void PlayBGM(AudioClip clip, bool loop = true)
    {
        if (bgmSource == null) return;
        bgmSource.clip = clip;
        bgmSource.loop = loop;
        bgmSource.Play();
    }

    public void PlayBattleBGM()
    {
        if (battleBgm != null)
            PlayBGM(battleBgm);
    }

    public void PlayDefaultBGM()
    {
        if (defaultBgm != null)
            PlayBGM(defaultBgm);
    }

    // BGM 정지
    public void StopBGM()
    {
        if (bgmSource.isPlaying)
            bgmSource.Stop();
    }
    //BGM 전환
    public IEnumerator FadeBGM(AudioClip newClip, float duration = 1f)
    {
        float startVolume = bgmSource.volume;

        // Fade out
        while (bgmSource.volume > 0)
        {
            bgmSource.volume -= startVolume * Time.unscaledDeltaTime / duration;
            yield return null;
        }

        bgmSource.Stop();
        bgmSource.clip = newClip;
        bgmSource.Play();

        // Fade in
        while (bgmSource.volume < startVolume)
        {
            bgmSource.volume += startVolume * Time.unscaledDeltaTime / duration;
            yield return null;
        }

        bgmSource.volume = startVolume;
    }

    // 효과음 재생
    public void PlaySFX(AudioClip clip)
    {
        if (sfxSource != null && clip != null)
        {
            sfxSource.PlayOneShot(clip);
        }
    }

    // UI 사운드 재생
    public void PlayUI(AudioClip clip)
    {
        if (uiSource != null && clip != null)
        {
            uiSource.PlayOneShot(clip);
        }
    }

    // 미리 등록된 효과음 재생
    public void PlayItemPickupSFX()
    {
        PlaySFX(itemPickupClip);
    }


    public void PlayDoorOpen()
    {
        PlaySFX(doorOpen);
    }

    public void PlayObjectiveGet()
    {
        PlaySFX(objectiveGet);
    }


    public void PlayFileOpen()
    {
        PlaySFX(fileOpen);
    }

    public void PlayInventoryOpen()
    {
        PlaySFX(fileOpen);
    }

    public void PlayMapOpen()
    {
        PlaySFX(mapOpen);
    }

    public void PlayButtonClick()
    {
        PlayUI(buttonClickClip);
    }

    //공격 효과음 랜덤 재생
    public void PlayRandomAttackSFX()
    {
        if (swordAttackClips.Length == 0) return;

        int index = Random.Range(0, swordAttackClips.Length);
        AudioClip selectedClip = swordAttackClips[index];

        sfxSource.pitch = Random.Range(0.95f, 1.05f); // 살짝 톤 변화
        PlaySFX(selectedClip);
        sfxSource.pitch = 1f; // 원래대로 복구
    }

    public void SetAndPlayDefaultBGM(AudioClip clip)
    {
        defaultBgm = clip;
        PlayBGM(defaultBgm);
    }
}
