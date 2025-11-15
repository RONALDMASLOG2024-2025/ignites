using UnityEngine;

public class MusicManager : MonoBehaviour
{
    public static MusicManager Instance { get; private set; }

    [Header("Music Tracks")]
    public AudioClip normalMusic;
    public AudioClip bossMusic;

    private AudioSource audioSource;
    private AudioClip currentlyPlaying;

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
        }

        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }

        audioSource.loop = true;
        audioSource.playOnAwake = false;
    }

    private void Start()
    {
        // Start with normal music
        if (normalMusic != null)
        {
            PlayNormalMusic();
        }
    }

    public void PlayNormalMusic()
    {
        if (normalMusic != null && currentlyPlaying != normalMusic)
        {
            audioSource.clip = normalMusic;
            audioSource.Play();
            currentlyPlaying = normalMusic;
            Debug.Log("Playing normal music");
        }
    }

    public void PlayBossMusic()
    {
        if (bossMusic != null && currentlyPlaying != bossMusic)
        {
            audioSource.clip = bossMusic;
            audioSource.Play();
            currentlyPlaying = bossMusic;
            Debug.Log("Playing BOSS music!");
        }
    }

    public void StopMusic()
    {
        audioSource.Stop();
        currentlyPlaying = null;
    }
}
