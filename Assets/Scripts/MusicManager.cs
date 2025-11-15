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
    [Tooltip("Scenes where game music should play (e.g., MainGame, Level2)")]
    public string[] gameScenes = { "MainGame", "Level2" };
    
    [Tooltip("Scenes where menu music should play (e.g., MainMenu)")]
    public string[] menuScenes = { "MainMenu" };

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
            PlayVictoryMusic();
        else
            StopMusic();
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        HandleSceneMusic(scene.name);
    }

    private void HandleSceneMusic(string sceneName)
    {
        // Check if this is a menu scene
        foreach (string menuScene in menuScenes)
        {
            if (sceneName == menuScene)
            {
                if (menuMusic != null && currentlyPlaying != menuMusic)
                {
                    PlayMenuMusic();
                }
                else if (menuMusic == null)
                {
                    StopMusic(); // Stop music if no menu music assigned
                }
                // If already playing menu music, do nothing (prevents double play)
                return;
            }
        }

        // Check if this is a game scene
        foreach (string gameScene in gameScenes)
        {
            if (sceneName == gameScene)
            {
                // Only play if not already playing normal music
                if (currentlyPlaying != normalMusic)
                {
                    PlayNormalMusic();
                }
                return;
            }
        }

        // If scene is not recognized, stop music
        Debug.LogWarning($"MusicManager: Scene '{sceneName}' not configured. Stopping music.");
        StopMusic();
    }

    public void PlayNormalMusic()
    {
        if (normalMusic != null && currentlyPlaying != normalMusic)
        {
            StartFadeToClip(normalMusic, true);
        }
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
        if (menuMusic != null && currentlyPlaying != menuMusic)
        {
            StartFadeToClip(menuMusic, true);
        }
    }

    public void StopMusic()
    {
        // Fade out then stop
        StartFadeToClip(null, true);
    }

    public void PlayVictoryMusic()
    {
        if (victoryMusic != null && currentlyPlaying != victoryMusic)
        {
            // Victory fanfare usually shouldn't loop
            StartFadeToClip(victoryMusic, false);
        }
    }

    private void StartFadeToClip(AudioClip target, bool loop = true)
    {
        if (fadeCoroutine != null)
        {
            StopCoroutine(fadeCoroutine);
        }
        fadeCoroutine = StartCoroutine(FadeToClipCoroutine(target, fadeDuration));
    }

    private System.Collections.IEnumerator FadeToClipCoroutine(AudioClip target, float duration)
    {
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
            Debug.Log("Music stopped (faded out)");
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

        // Fade in
        time = 0f;
        while (time < duration)
        {
            time += Time.unscaledDeltaTime;
            audioSource.volume = Mathf.Lerp(0f, 1f, time / duration);
            yield return null;
        }

        audioSource.volume = 1f;
    }
}
