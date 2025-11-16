using UnityEngine;
using UnityEngine.SceneManagement;

public class MusicManager : MonoBehaviour
{
    public static MusicManager Instance { get; private set; }

    [Header("Music Tracks")]
    public AudioClip normalMusic;
    public AudioClip bossMusic;
    public AudioClip victoryMusic; // short fanfare to play on victory (non-loop)
    public AudioClip menuMusic; // Optional: separate music for main menu

    [Header("Scene Settings")]
    [Tooltip("Automatically detects scenes starting with 'Level'. Add custom scenes here if needed.")]
    public string[] gameScenes = { "Level1", "Level2", "BossLevel" };
    
    [Tooltip("Scenes where menu music should play (e.g., MainMenu)")]
    public string[] menuScenes = { "MainMenu" };
    
    [Tooltip("Auto-detect all scenes starting with 'Level' (recommended)")]
    public bool autoDetectLevelScenes = true;

    private AudioSource audioSource;
    private AudioClip currentlyPlaying;
    [Header("Fade Settings")]
    [Tooltip("Seconds to crossfade between tracks")]
    public float fadeDuration = 1.0f;
    private Coroutine fadeCoroutine;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // keeps music playing between scenes
        }
        else
        {
            Destroy(gameObject); // prevent duplicate managers
            return;
        }

        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }

        audioSource.loop = true;
        audioSource.playOnAwake = false;

        // Subscribe to scene changes to handle music switching
        SceneManager.sceneLoaded += OnSceneLoaded;
        
        // Subscribe to GameState events for graceful shutdown
        if (GameState.Instance != null)
        {
            GameState.Instance.OnGameOver += HandleGameOver;
            GameState.Instance.OnVictory += HandleVictory;
        }
    }

    private void Start()
    {
        // Play appropriate music for the starting scene
        HandleSceneMusic(SceneManager.GetActiveScene().name);

        // Ensure we are subscribed to GameState events even if GameState was created
        // after Awake (UIManager may create GameState in its Start()).
        if (GameState.Instance != null)
        {
            // Unsubscribe first to avoid duplicate subscriptions
            GameState.Instance.OnGameOver -= HandleGameOver;
            GameState.Instance.OnVictory -= HandleVictory;
            GameState.Instance.OnGameOver += HandleGameOver;
            GameState.Instance.OnVictory += HandleVictory;
        }
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
        
        // Unsubscribe from GameState events
        if (GameState.Instance != null)
        {
            GameState.Instance.OnGameOver -= HandleGameOver;
            GameState.Instance.OnVictory -= HandleVictory;
        }
    }

    private void HandleGameOver()
    {
        Debug.Log("MusicManager: Game Over event received. Fading out music.");
        StopMusic();
    }

    private void HandleVictory()
    {
        Debug.Log("MusicManager: Victory event received. Playing victory fanfare.");
        // Play victory fanfare (non-looping) if assigned, otherwise fade out
        if (victoryMusic != null)
        {
            PlayVictoryMusic();
        }
        else
        {
            // If no victory music, clear the current track so next level music will play
            currentlyPlaying = null;
            StopMusic();
        }
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // Stop any running victory music cleanup coroutines
        StopAllCoroutines();
        
        // Handle scene music
        HandleSceneMusic(scene.name);
    }

    private void HandleSceneMusic(string sceneName)
    {
        Debug.Log($"MusicManager: Scene loaded: '{sceneName}'. Current track: {(currentlyPlaying != null ? currentlyPlaying.name : "None")}, IsPlaying: {audioSource.isPlaying}");
        
        // Check if this is a menu scene
        foreach (string menuScene in menuScenes)
        {
            if (sceneName == menuScene)
            {
                if (menuMusic != null)
                {
                    // Always play menu music when entering menu
                    PlayMenuMusic();
                }
                else
                {
                    StopMusic(); // Stop music if no menu music assigned
                }
                return;
            }
        }

        // Check if this is a game scene
        // First check explicit gameScenes array
        bool isGameScene = false;
        foreach (string gameScene in gameScenes)
        {
            if (sceneName == gameScene)
            {
                isGameScene = true;
                break;
            }
        }
        
        // If auto-detect is enabled, also check if scene name starts with "Level"
        if (!isGameScene && autoDetectLevelScenes && sceneName.StartsWith("Level"))
        {
            isGameScene = true;
            Debug.Log($"MusicManager: Auto-detected '{sceneName}' as a game level");
        }
        
        if (isGameScene)
        {
            // Always restart normal music when entering a game level
            Debug.Log($"MusicManager: Game scene detected. Force playing normal music.");
            PlayNormalMusic();
            return;
        }

        // If scene is not recognized, stop music
        Debug.LogWarning($"MusicManager: Scene '{sceneName}' not configured. Stopping music.");
        StopMusic();
    }

    public void PlayNormalMusic()
    {
        if (normalMusic == null)
        {
            Debug.LogError("MusicManager: Normal music clip is NOT ASSIGNED! Please assign it in the Inspector.");
            return;
        }
        
        if (audioSource == null)
        {
            Debug.LogError("MusicManager: AudioSource is null! This should never happen.");
            return;
        }
        
        Debug.Log($"MusicManager: Playing normal music '{normalMusic.name}'");
        StartFadeToClip(normalMusic, true);
    }

    public void PlayBossMusic()
    {
        if (bossMusic != null && currentlyPlaying != bossMusic)
        {
            StartFadeToClip(bossMusic, true);
        }
    }

    public void PlayMenuMusic()
    {
        if (menuMusic != null)
        {
            Debug.Log("MusicManager: Playing menu music");
            StartFadeToClip(menuMusic, true);
        }
        else
        {
            Debug.LogWarning("MusicManager: Menu music clip not assigned!");
        }
    }

    public void StopMusic()
    {
        // Fade out then stop
        currentlyPlaying = null; // Clear the reference immediately
        StartFadeToClip(null, true);
    }

    public void PlayVictoryMusic()
    {
        if (victoryMusic != null)
        {
            // Victory fanfare usually shouldn't loop
            StartFadeToClip(victoryMusic, false);
            // Clear currentlyPlaying after victory music so next level will start fresh
            StartCoroutine(ClearCurrentTrackAfterVictory());
        }
    }
    
    /// <summary>
    /// Clears the current track reference after victory music finishes
    /// so that the next level's music will play properly
    /// </summary>
    private System.Collections.IEnumerator ClearCurrentTrackAfterVictory()
    {
        // Wait for victory music to finish (check its length)
        if (victoryMusic != null)
        {
            yield return new WaitForSeconds(victoryMusic.length + fadeDuration);
        }
        
        // Clear the current track so next scene music will play
        currentlyPlaying = null;
        Debug.Log("MusicManager: Victory music finished, cleared current track");
    }

    private void StartFadeToClip(AudioClip target, bool loop = true)
    {
        // If already transitioning to the same clip, don't restart the fade
        if (fadeCoroutine != null && currentlyPlaying == target && target != null)
        {
            Debug.Log($"MusicManager: Already fading to {target.name}, skipping duplicate fade");
            return;
        }
        
        if (fadeCoroutine != null)
        {
            StopCoroutine(fadeCoroutine);
        }
        fadeCoroutine = StartCoroutine(FadeToClipCoroutine(target, fadeDuration));
    }

    private System.Collections.IEnumerator FadeToClipCoroutine(AudioClip target, float duration)
    {
        Debug.Log($"MusicManager: Starting fade to '{(target != null ? target.name : "NULL")}'. Duration: {duration}s");
        
        float startVol = audioSource.volume;
        float time = 0f;

        // Fade out current
        while (time < duration)
        {
            time += Time.unscaledDeltaTime;
            audioSource.volume = Mathf.Lerp(startVol, 0f, time / duration);
            yield return null;
        }

        audioSource.volume = 0f;

        if (target == null)
        {
            audioSource.Stop();
            currentlyPlaying = null;
            Debug.Log("MusicManager: Music stopped (faded out)");
            yield break;
        }

        // Switch clip
        audioSource.clip = target;
        // If the target clip is the victory fanfare and you don't want it to loop,
        // keep the AudioSource.loop setting false. Otherwise keep looping enabled.
        // We'll set loop based on whether the clip matches victoryMusic.
        audioSource.loop = (target == victoryMusic) ? false : true;
        audioSource.Play();
        currentlyPlaying = target;
        
        Debug.Log($"MusicManager: Now playing '{target.name}'. Loop: {audioSource.loop}, IsPlaying: {audioSource.isPlaying}");

        // Fade in
        time = 0f;
        while (time < duration)
        {
            time += Time.unscaledDeltaTime;
            audioSource.volume = Mathf.Lerp(0f, 1f, time / duration);
            yield return null;
        }

        audioSource.volume = 1f;
        Debug.Log($"MusicManager: Fade complete. Volume: {audioSource.volume}, IsPlaying: {audioSource.isPlaying}");
    }
}
