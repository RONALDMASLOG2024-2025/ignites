using System;
using UnityEngine;

/// <summary>
/// Centralized game state and pause controller.
/// Add this to a persistent GameObject (created automatically if missing).
/// Other systems should query GameState.Instance.IsPaused / IsGameEnded instead of
/// checking Time.timeScale directly.
/// </summary>
public class GameState : MonoBehaviour
{
    public static GameState Instance { get; private set; }

    public bool IsPaused { get; private set; }
    public bool IsGameOver { get; private set; }
    public bool IsVictory { get; private set; }
    public bool IsGameEnded => IsGameOver || IsVictory;

    // Events for systems to subscribe and perform graceful shutdown/cleanup
    public event Action<bool> OnPauseChanged; // parameter: isPaused
    public event Action OnGameOver;
    public event Action OnVictory;
    public event Action OnGameReset; // fired when ResetState is called (scene reload, etc.)

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void Pause()
    {
        if (IsPaused) return;
        IsPaused = true;
        Time.timeScale = 0f;
        OnPauseChanged?.Invoke(true);
        StopAllSFX();
    }

    public void Resume()
    {
        if (!IsPaused) return;
        IsPaused = false;
        Time.timeScale = 1f;
        OnPauseChanged?.Invoke(false);
    }

    public void SetGameOver()
    {
        if (IsGameOver) return;
        IsGameOver = true;
        IsPaused = true;
        Time.timeScale = 0f;
        StopAllSFX();
        OnGameOver?.Invoke();
    }

    public void SetVictory()
    {
        if (IsVictory) return;
        IsVictory = true;
        IsPaused = true;
        Time.timeScale = 0f;
        StopAllSFX();
        OnVictory?.Invoke();
    }

    public void ResetState()
    {
        IsPaused = false;
        IsGameOver = false;
        IsVictory = false;
        Time.timeScale = 1f;
        OnGameReset?.Invoke();
        Debug.Log("GameState reset (for scene reload/restart).");
    }

    /// <summary>
    /// Stops all SFX audio sources (footsteps, attack sounds, pickups).
    /// Does not affect music (MusicManager handles its own audio).
    /// </summary>
    private void StopAllSFX()
    {
        // Find all AudioSources in the scene
        AudioSource[] allAudioSources = FindObjectsByType<AudioSource>(FindObjectsSortMode.None);
        
        foreach (AudioSource source in allAudioSources)
        {
            // Skip the MusicManager's AudioSource (it handles its own state)
            if (MusicManager.Instance != null && source.transform.IsChildOf(MusicManager.Instance.transform))
                continue;
            if (MusicManager.Instance != null && source.gameObject == MusicManager.Instance.gameObject)
                continue;
            
            // Stop all other audio sources (enemy footsteps, attacks, pickups, etc.)
            if (source.isPlaying)
            {
                source.Stop();
            }
        }
    }
}
