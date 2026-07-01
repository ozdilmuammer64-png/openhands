using UnityEngine;
using System.Collections.Generic;

namespace KnightOnline.Scripts
{
    [System.Serializable]
    public class SoundEffect
    {
        public string name;
        public AudioClip clip;
        [Range(0, 1)]
        public float volume = 1f;
        [Range(0.5f, 2f)]
        public float pitch = 1f;
        public bool loop = false;
    }
    
    public class AudioSystem : MonoBehaviour
    {
        public static AudioSystem Instance { get; private set; }
        
        [Header("Audio Sources")]
        public AudioSource musicSource;
        public AudioSource sfxSource;
        public AudioSource ambientSource;
        
        [Header("Sound Effects")]
        public List<SoundEffect> soundEffects = new List<SoundEffect>();
        
        [Header("Settings")]
        public float masterVolume = 1f;
        public float musicVolume = 0.7f;
        public float sfxVolume = 1f;
        public bool muteMusic = false;
        public bool muteSFX = false;
        
        [Header("Music Tracks")]
        public AudioClip mainMenuMusic;
        public AudioClip townMusic;
        public AudioClip battleMusic;
        public AudioClip bossMusic;
        public AudioClip victoryMusic;
        
        private Dictionary<string, AudioClip> soundDictionary = new Dictionary<string, AudioClip>();
        private AudioClip currentMusic;
        
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
            
            SetupAudioSources();
            BuildSoundDictionary();
        }
        
        void SetupAudioSources()
        {
            if (musicSource == null)
            {
                GameObject musicObj = new GameObject("MusicSource");
                musicObj.transform.parent = transform;
                musicSource = musicObj.AddComponent<AudioSource>();
                musicSource.loop = true;
                musicSource.playOnAwake = false;
            }
            
            if (sfxSource == null)
            {
                GameObject sfxObj = new GameObject("SFXSource");
                sfxObj.transform.parent = transform;
                sfxSource = sfxObj.AddComponent<AudioSource>();
                sfxSource.loop = false;
                sfxSource.playOnAwake = false;
            }
            
            if (ambientSource == null)
            {
                GameObject ambientObj = new GameObject("AmbientSource");
                ambientObj.transform.parent = transform;
                ambientSource = ambientObj.AddComponent<AudioSource>();
                ambientSource.loop = true;
                ambientSource.playOnAwake = false;
            }
        }
        
        void BuildSoundDictionary()
        {
            soundDictionary.Clear();
            foreach (SoundEffect sfx in soundEffects)
            {
                if (sfx.clip != null)
                {
                    soundDictionary[sfx.name] = sfx.clip;
                }
            }
        }
        
        public void PlayMusic(AudioClip music, float fadeDuration = 1f)
        {
            if (music == null || music == currentMusic) return;
            
            StartCoroutine(FadeMusic(music, fadeDuration));
        }
        
        System.Collections.IEnumerator FadeMusic(AudioClip newMusic, float duration)
        {
            float startVolume = musicSource.volume;
            
            // Fade out
            float elapsed = 0;
            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                musicSource.volume = Mathf.Lerp(startVolume, 0, elapsed / duration);
                yield return null;
            }
            
            // Switch music
            musicSource.clip = newMusic;
            musicSource.Play();
            currentMusic = newMusic;
            
            // Fade in
            elapsed = 0;
            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                musicSource.volume = Mathf.Lerp(0, GetMusicVolume(), elapsed / duration);
                yield return null;
            }
        }
        
        public void PlaySound(string soundName, Vector3 position = default)
        {
            if (muteSFX) return;
            
            if (soundDictionary.TryGetValue(soundName, out AudioClip clip))
            {
                if (position != default)
                {
                    AudioSource.PlayClipAtPoint(clip, position, GetSFXVolume());
                }
                else
                {
                    sfxSource.PlayOneShot(clip, GetSFXVolume());
                }
            }
        }
        
        public void PlaySound(AudioClip clip, Vector3 position = default)
        {
            if (muteSFX || clip == null) return;
            
            if (position != default)
            {
                AudioSource.PlayClipAtPoint(clip, position, GetSFXVolume());
            }
            else
            {
                sfxSource.PlayOneShot(clip, GetSFXVolume());
            }
        }
        
        // Combat sounds
        public void PlayAttackSound()
        {
            PlaySound("Attack", transform.position);
        }
        
        public void PlayHitSound()
        {
            PlaySound("Hit", transform.position);
        }
        
        public void PlayDeathSound()
        {
            PlaySound("Death", transform.position);
        }
        
        public void PlayPickupSound()
        {
            PlaySound("Pickup", transform.position);
        }
        
        public void PlayLevelUpSound()
        {
            PlaySound("LevelUp", transform.position);
        }
        
        public void PlaySkillSound(string skillName)
        {
            PlaySound($"Skill_{skillName}", transform.position);
        }
        
        // Music zones
        public void PlayTownMusic()
        {
            PlayMusic(townMusic);
        }
        
        public void PlayBattleMusic()
        {
            PlayMusic(battleMusic);
        }
        
        public void PlayBossMusic()
        {
            PlayMusic(bossMusic);
        }
        
        public void PlayVictoryMusic()
        {
            PlayMusic(victoryMusic, 0.5f);
        }
        
        // Volume controls
        float GetMusicVolume()
        {
            return masterVolume * musicVolume * (muteMusic ? 0 : 1);
        }
        
        float GetSFXVolume()
        {
            return masterVolume * sfxVolume * (muteSFX ? 0 : 1);
        }
        
        public void SetMasterVolume(float volume)
        {
            masterVolume = Mathf.Clamp01(volume);
            UpdateVolumes();
        }
        
        public void SetMusicVolume(float volume)
        {
            musicVolume = Mathf.Clamp01(volume);
            UpdateVolumes();
        }
        
        public void SetSFXVolume(float volume)
        {
            sfxVolume = Mathf.Clamp01(volume);
            UpdateVolumes();
        }
        
        public void ToggleMusic()
        {
            muteMusic = !muteMusic;
            UpdateVolumes();
        }
        
        public void ToggleSFX()
        {
            muteSFX = !muteSFX;
        }
        
        void UpdateVolumes()
        {
            musicSource.volume = GetMusicVolume();
            sfxSource.volume = GetSFXVolume();
            ambientSource.volume = GetSFXVolume() * 0.5f;
        }
    }
}
