using UnityEngine;
using System.Collections.Generic;
using SortingBoardGame.Data;

namespace SortingBoardGame.Managers
{
    public class AudioManager : MonoBehaviour
    {
        public static AudioManager Instance { get; private set; }
        
        [Header("Audio Sources")]
        [SerializeField] private AudioSource musicSource;
        [SerializeField] private AudioSource sfxSource;
        
        [Header("Audio Clips")]
        [SerializeField] private AudioClip backgroundMusic;
        [SerializeField] private AudioClip successPopSound;
        [SerializeField] private AudioClip failureBuzzSound;
        [SerializeField] private AudioClip ballRollSound;
        
        [Header("Audio Settings")]
        [SerializeField] private float musicVolume = 0.5f;
        [SerializeField] private float sfxVolume = 0.8f;
        [SerializeField] private bool musicMuted = false;
        [SerializeField] private bool sfxMuted = false;
        
        // Audio object pooling for performance
        private Queue<AudioSource> audioSourcePool = new Queue<AudioSource>();
        private List<AudioSource> activeAudioSources = new List<AudioSource>();
        
        void Awake()
        {
            // Singleton pattern
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
                InitializeAudioManager();
            }
            else
            {
                Destroy(gameObject);
            }
        }
        
        void Start()
        {
            CreateDefaultAudioClips();
            StartBackgroundMusic();
        }
        
        private void InitializeAudioManager()
        {
            // Create audio sources if not assigned
            if (musicSource == null)
            {
                GameObject musicObj = new GameObject("MusicSource");
                musicObj.transform.SetParent(transform);
                musicSource = musicObj.AddComponent<AudioSource>();
                musicSource.loop = true;
                musicSource.playOnAwake = false;
                musicSource.volume = musicVolume;
            }
            
            if (sfxSource == null)
            {
                GameObject sfxObj = new GameObject("SFXSource");
                sfxObj.transform.SetParent(transform);
                sfxSource = sfxObj.AddComponent<AudioSource>();
                sfxSource.loop = false;
                sfxSource.playOnAwake = false;
                sfxSource.volume = sfxVolume;
            }
            
            // Initialize audio source pool for 3D positional audio
            InitializeAudioSourcePool();
            
            Debug.Log("AudioManager initialized");
        }
        
        private void InitializeAudioSourcePool()
        {
            // Create pool of audio sources for positional sound effects
            for (int i = 0; i < 10; i++)
            {
                GameObject audioObj = new GameObject($"PooledAudioSource_{i}");
                audioObj.transform.SetParent(transform);
                AudioSource audioSource = audioObj.AddComponent<AudioSource>();
                audioSource.playOnAwake = false;
                audioSource.spatialBlend = 1.0f; // 3D sound
                audioSource.volume = sfxVolume;
                audioObj.SetActive(false);
                
                audioSourcePool.Enqueue(audioSource);
            }
        }
        
        private void CreateDefaultAudioClips()
        {
            // Create simple procedural audio clips if none are assigned
            if (successPopSound == null)
            {
                successPopSound = CreatePopSound();
            }
            
            if (failureBuzzSound == null)
            {
                failureBuzzSound = CreateBuzzSound();
            }
            
            if (ballRollSound == null)
            {
                ballRollSound = CreateRollSound();
            }
            
            if (backgroundMusic == null)
            {
                backgroundMusic = CreateBackgroundMusic();
            }
        }
        
        public void StartBackgroundMusic()
        {
            if (musicSource != null && backgroundMusic != null && !musicMuted)
            {
                musicSource.clip = backgroundMusic;
                musicSource.Play();
                Debug.Log("Background music started");
            }
        }
        
        public void StopBackgroundMusic()
        {
            if (musicSource != null && musicSource.isPlaying)
            {
                musicSource.Stop();
                Debug.Log("Background music stopped");
            }
        }
        
        public void ToggleMusicMute()
        {
            musicMuted = !musicMuted;
            
            if (musicMuted)
            {
                StopBackgroundMusic();
            }
            else
            {
                StartBackgroundMusic();
            }
            
            Debug.Log($"Music muted: {musicMuted}");
        }
        
        public void PlaySuccessSound(Vector3 position)
        {
            if (successPopSound != null && !sfxMuted)
            {
                PlayPositionalSound(successPopSound, position);
                Debug.Log($"Playing success 'pop' sound at {position}");
            }
        }
        
        public void PlayFailureSound(Vector3 position)
        {
            if (failureBuzzSound != null && !sfxMuted)
            {
                PlayPositionalSound(failureBuzzSound, position);
                Debug.Log($"Playing failure 'buzz' sound at {position}");
            }
        }
        
        public void PlayBallRollSound(Vector3 position, float intensity = 1.0f)
        {
            if (ballRollSound != null && !sfxMuted)
            {
                AudioSource audioSource = PlayPositionalSound(ballRollSound, position);
                if (audioSource != null)
                {
                    audioSource.volume = sfxVolume * intensity * 0.3f; // Quieter for rolling
                }
            }
        }
        
        private AudioSource PlayPositionalSound(AudioClip clip, Vector3 position)
        {
            AudioSource audioSource = GetPooledAudioSource();
            if (audioSource != null)
            {
                audioSource.transform.position = position;
                audioSource.clip = clip;
                audioSource.volume = sfxVolume;
                audioSource.Play();
                
                // Return to pool after clip finishes
                StartCoroutine(ReturnToPoolAfterClip(audioSource, clip.length));
                
                return audioSource;
            }
            else
            {
                // Fallback to non-positional sound
                sfxSource.PlayOneShot(clip, sfxVolume);
                return sfxSource;
            }
        }
        
        private AudioSource GetPooledAudioSource()
        {
            if (audioSourcePool.Count > 0)
            {
                AudioSource audioSource = audioSourcePool.Dequeue();
                audioSource.gameObject.SetActive(true);
                activeAudioSources.Add(audioSource);
                return audioSource;
            }
            
            return null;
        }
        
        private System.Collections.IEnumerator ReturnToPoolAfterClip(AudioSource audioSource, float clipLength)
        {
            yield return new WaitForSeconds(clipLength + 0.1f);
            
            if (audioSource != null)
            {
                audioSource.Stop();
                audioSource.gameObject.SetActive(false);
                activeAudioSources.Remove(audioSource);
                audioSourcePool.Enqueue(audioSource);
            }
        }
        
        // Procedural audio generation for default sounds
        private AudioClip CreatePopSound()
        {
            int sampleRate = 44100;
            float duration = 0.2f;
            int samples = Mathf.RoundToInt(sampleRate * duration);
            
            float[] audioData = new float[samples];
            
            for (int i = 0; i < samples; i++)
            {
                float time = (float)i / sampleRate;
                float envelope = Mathf.Exp(-time * 8f); // Quick decay
                float frequency = 800f + 400f * Mathf.Exp(-time * 10f); // Pitch sweep down
                audioData[i] = envelope * Mathf.Sin(2f * Mathf.PI * frequency * time) * 0.3f;
            }
            
            AudioClip clip = AudioClip.Create("PopSound", samples, 1, sampleRate, false);
            clip.SetData(audioData, 0);
            return clip;
        }
        
        private AudioClip CreateBuzzSound()
        {
            int sampleRate = 44100;
            float duration = 0.3f;
            int samples = Mathf.RoundToInt(sampleRate * duration);
            
            float[] audioData = new float[samples];
            
            for (int i = 0; i < samples; i++)
            {
                float time = (float)i / sampleRate;
                float envelope = Mathf.Exp(-time * 3f); // Slower decay
                float frequency = 150f; // Low buzz frequency
                audioData[i] = envelope * Mathf.Sin(2f * Mathf.PI * frequency * time) * 0.2f;
            }
            
            AudioClip clip = AudioClip.Create("BuzzSound", samples, 1, sampleRate, false);
            clip.SetData(audioData, 0);
            return clip;
        }
        
        private AudioClip CreateRollSound()
        {
            int sampleRate = 44100;
            float duration = 1.0f;
            int samples = Mathf.RoundToInt(sampleRate * duration);
            
            float[] audioData = new float[samples];
            
            for (int i = 0; i < samples; i++)
            {
                float time = (float)i / sampleRate;
                // Create rolling noise using multiple frequencies
                float noise = 0f;
                for (int freq = 100; freq < 1000; freq += 100)
                {
                    noise += Mathf.Sin(2f * Mathf.PI * freq * time) * Random.Range(0.1f, 0.3f);
                }
                audioData[i] = noise * 0.1f;
            }
            
            AudioClip clip = AudioClip.Create("RollSound", samples, 1, sampleRate, false);
            clip.SetData(audioData, 0);
            return clip;
        }
        
        private AudioClip CreateBackgroundMusic()
        {
            int sampleRate = 44100;
            float duration = 30.0f; // 30 second loop
            int samples = Mathf.RoundToInt(sampleRate * duration);
            
            float[] audioData = new float[samples];
            
            // Simple pleasant melody
            float[] melody = { 261.63f, 293.66f, 329.63f, 349.23f, 392.00f, 440.00f, 493.88f, 523.25f }; // C major scale
            
            for (int i = 0; i < samples; i++)
            {
                float time = (float)i / sampleRate;
                int noteIndex = Mathf.FloorToInt(time * 2) % melody.Length; // Change note every 0.5 seconds
                float frequency = melody[noteIndex];
                
                float envelope = 0.5f + 0.3f * Mathf.Sin(2f * Mathf.PI * time * 0.1f); // Gentle volume variation
                audioData[i] = envelope * Mathf.Sin(2f * Mathf.PI * frequency * time) * 0.1f;
            }
            
            AudioClip clip = AudioClip.Create("BackgroundMusic", samples, 1, sampleRate, false);
            clip.SetData(audioData, 0);
            return clip;
        }
        
        // Public properties for UI controls
        public bool IsMusicMuted => musicMuted;
        public bool IsSFXMuted => sfxMuted;
        public float MusicVolume => musicVolume;
        public float SFXVolume => sfxVolume;
        
        public void SetMusicVolume(float volume)
        {
            musicVolume = Mathf.Clamp01(volume);
            if (musicSource != null)
            {
                musicSource.volume = musicVolume;
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
        
        public void ToggleSFXMute()
        {
            sfxMuted = !sfxMuted;
            Debug.Log($"SFX muted: {sfxMuted}");
        }
    }
}