using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

/// <summary>
/// Simple UI manager for in-game panels: HUD, Pause, Game Over.
/// Attach to a persistent UI GameObject in the scene (e.g., Canvas > UIManager).
/// Wire the panel GameObjects in the inspector.
/// </summary>
public class UIManager : MonoBehaviour
{
    [Header("Panels")]
    public GameObject hudPanel; // health, enemy count, etc.
    public GameObject pausePanel;
    public GameObject gameOverPanel;
    public GameObject victoryPanel;
    public GameObject finalVictoryPanel; // Special panel for beating the final level
    
    [Header("Victory / Level")]
    public string nextLevelSceneName = ""; // set in inspector for Level1 -> Level2. Leave empty for final level.
    
    [Header("Final Victory")]
    [TextArea(3, 6)]
    public string finalVictoryMessage = "Congratulations!\n\nYou have defeated all enemies and saved the kingdom!\n\nThank you for playing!";

    [Header("HUD Elements")]
    public TMP_Text enemyCountText;
    public TMP_Text healthText;

    private bool isPaused = false;
    // NOTE: Pause/end state is now centralized in GameState. We still keep a local
    // isPaused for UI convenience but will defer behavior to GameState when present.

    private void Start()
    {
        // Ensure correct initial state
        HideAllPanels();
        if (hudPanel != null) hudPanel.SetActive(true);
        // Ensure GameState exists so other systems can rely on it
        if (GameState.Instance == null)
        {
            new GameObject("GameState").AddComponent<GameState>();
        }
        GameState.Instance.ResetState();
        isPaused = false;
    }

    private void Update()
    {
        // Toggle pause with Escape key (only when not blocked by game end)
        if (GameState.Instance != null)
        {
            if (!GameState.Instance.IsGameEnded && Input.GetKeyDown(KeyCode.Escape))
            {
                TogglePause();
            }
        }
        else
        {
            // Fallback: if GameState isn't present, still allow pause toggle
            if (Input.GetKeyDown(KeyCode.Escape)) TogglePause();
        }
    }

    public void TogglePause()
    {
        if (isPaused) ResumeGame(); else PauseGame();
    }

    public void PauseGame()
    {
        if (pausePanel != null) pausePanel.SetActive(true);
        if (hudPanel != null) hudPanel.SetActive(false);
        // Use GameState to centralize pause state and Time.timeScale management
        if (GameState.Instance != null)
        {
            GameState.Instance.Pause();
        }
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void ResumeGame()
    {
        if (pausePanel != null) pausePanel.SetActive(false);
        if (hudPanel != null) hudPanel.SetActive(true);
        // Use GameState to centralize resume state and Time.timeScale management
        if (GameState.Instance != null)
        {
            GameState.Instance.Resume();
        }
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    public void ShowGameOver()
    {
        if (gameOverPanel != null) gameOverPanel.SetActive(true);
        if (hudPanel != null) hudPanel.SetActive(false);
        // Delegate to GameState which will broadcast to all subscribers
        if (GameState.Instance != null)
        {
            GameState.Instance.SetGameOver();
        }
        // Ensure player can use the cursor to click UI buttons
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void ShowVictory()
    {
        // Check if this is the final level (no next level scene set)
        if (string.IsNullOrEmpty(nextLevelSceneName))
        {
            ShowFinalVictory();
        }
        else
        {
            // Regular victory with Next Level button
            if (victoryPanel != null) victoryPanel.SetActive(true);
            if (hudPanel != null) hudPanel.SetActive(false);
            // Delegate to GameState which will broadcast to all subscribers
            if (GameState.Instance != null)
            {
                GameState.Instance.SetVictory();
            }
        }
    }

    /// <summary>
    /// Shows the final victory panel when the player beats the last level.
    /// This is different from regular victory because there's no Next Level button.
    /// </summary>
    public void ShowFinalVictory()
    {
        Debug.Log("🎉 FINAL VICTORY! Player beat the game!");
        
        if (finalVictoryPanel != null)
        {
            finalVictoryPanel.SetActive(true);
            
            // Update final victory message if there's a text component
            TMP_Text messageText = finalVictoryPanel.GetComponentInChildren<TMP_Text>();
            if (messageText != null && !string.IsNullOrEmpty(finalVictoryMessage))
            {
                messageText.text = finalVictoryMessage;
            }
        }
        else
        {
            // Fallback: use regular victory panel but hide Next Level button
            Debug.LogWarning("UIManager: finalVictoryPanel not assigned. Using regular victoryPanel.");
            if (victoryPanel != null) victoryPanel.SetActive(true);
        }
        
        if (hudPanel != null) hudPanel.SetActive(false);
        // Allow mouse cursor for clicking final victory UI
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        // Delegate to GameState which will broadcast to all subscribers
        if (GameState.Instance != null)
        {
            GameState.Instance.SetVictory();
        }
    }

    public void HideAllPanels()
    {
        if (hudPanel != null) hudPanel.SetActive(false);
        if (pausePanel != null) pausePanel.SetActive(false);
        if (gameOverPanel != null) gameOverPanel.SetActive(false);
        if (victoryPanel != null) victoryPanel.SetActive(false);
        if (finalVictoryPanel != null) finalVictoryPanel.SetActive(false);
    }

    // UI button hooks
    public void OnResumeButton()
    {
        ResumeGame();
    }

    public void OnRestartButton()
    {
        // Reset GameState (broadcasts OnGameReset event to subscribers)
        if (GameState.Instance != null)
        {
            GameState.Instance.ResetState();
        }
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void OnQuitToMenuButton()
    {
        // Reset GameState (broadcasts OnGameReset event to subscribers)
        if (GameState.Instance != null)
        {
            GameState.Instance.ResetState();
        }
        SceneManager.LoadScene("MainMenu");
    }

    // Called by Victory panel Next Level button
    public void OnNextLevelButton()
    {
        // Reset GameState (broadcasts OnGameReset event to subscribers)
        if (GameState.Instance != null)
        {
            GameState.Instance.ResetState();
        }
        if (!string.IsNullOrEmpty(nextLevelSceneName))
        {
            SceneManager.LoadScene(nextLevelSceneName);
        }
        else
        {
            Debug.LogWarning("UIManager: Next level scene name not set in inspector.");
        }
    }

    // HUD helpers
    public void SetEnemyCount(int n)
    {
        if (enemyCountText != null) enemyCountText.text = $"Enemies: {n}";
    }

    public void SetHealth(int current, int max)
    {
        if (healthText != null) healthText.text = $"Life: {current}/{max}";
    }
}
