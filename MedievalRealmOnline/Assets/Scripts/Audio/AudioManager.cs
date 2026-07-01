using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    [Header("Audio Sources")]
    public AudioSource bgmSource;
    public AudioSource sfxSource;

    [Header("BGM")]
    public AudioClip mainMenuBgm;
    public AudioClip battleBgm;
    public AudioClip villageBgm;

    [Header("SFX")]
    public AudioClip attackSound;
    public AudioClip hitSound;
    public AudioClip levelUpSound;
    public AudioClip pickupSound;
    public AudioClip deathSound;
    public AudioClip healSound;
    public AudioClip skillSound;

    [Header("Settings")]
    [Range(0, 1)]
    public float bgmVolume = 0.5f;
    [Range(0, 1)]
    public float sfxVolume = 0.7f;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        // Setup audio sources if not assigned
        if (bgmSource == null)
        {
            bgmSource = gameObject.AddComponent<AudioSource>();
            bgmSource.loop = true;
            bgmSource.playOnAwake = true;
        }

        if (sfxSource == null)
        {
            sfxSource = gameObject.AddComponent<AudioSource>();
        }

        bgmSource.volume = bgmVolume;
        sfxSource.volume = sfxVolume;
    }

    void Start()
    {
        PlayBGM(mainMenuBgm);
    }

    public void PlayBGM(AudioClip clip)
    {
        if (bgmSource != null && clip != null)
        {
            bgmSource.clip = clip;
            bgmSource.Play();
        }
    }

    public void PlaySFX(AudioClip clip)
    {
        if (sfxSource != null && clip != null)
        {
            sfxSource.PlayOneShot(clip);
        }
    }

    public void PlayAttackSound()
    {
        PlaySFX(attackSound);
    }

    public void PlayHitSound()
    {
        PlaySFX(hitSound);
    }

    public void PlayLevelUpSound()
    {
        PlaySFX(levelUpSound);
    }

    public void PlayPickupSound()
    {
        PlaySFX(pickupSound);
    }

    public void PlayDeathSound()
    {
        PlaySFX(deathSound);
    }

    public void PlayHealSound()
    {
        PlaySFX(healSound);
    }

    public void PlaySkillSound()
    {
        PlaySFX(skillSound);
    }

    public void SetBGMVolume(float volume)
    {
        bgmVolume = Mathf.Clamp01(volume);
        if (bgmSource != null)
        {
            bgmSource.volume = bgmVolume;
        }
    }

    public void SetSFXVolume(float volume)
    {
        sfxVolume = Mathf.Clamp01(volume);
        if (sfxSource != null)
        {
            sfxSource.volume = sfxVolume;
        }
    }

    public void MuteBGM(bool mute)
    {
        if (bgmSource != null)
        {
            bgmSource.mute = mute;
        }
    }

    public void MuteSFX(bool mute)
    {
        if (sfxSource != null)
        {
            sfxSource.mute = mute;
        }
    }
}