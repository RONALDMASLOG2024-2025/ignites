using UnityEngine;

/// <summary>
/// Debug helper for managing game progress during development.
/// Attach to any GameObject or call via console commands.
/// </summary>
public class ProgressDebugger : MonoBehaviour
{
    [Header("Quick Actions")]
    [Tooltip("Reset all progress when this is checked in Inspector")]
    public bool resetProgressNow = false;
    
    [Tooltip("Show progress details in console")]
    public bool showProgressInfo = false;

    private void Update()
    {
        // Reset progress if checkbox is checked
        if (resetProgressNow)
        {
            resetProgressNow = false;
            ResetProgress();
        }
        
        // Show progress info if checkbox is checked
        if (showProgressInfo)
        {
            showProgressInfo = false;
            ShowProgressInfo();
        }
    }

    /// <summary>
    /// Reset all game progress (levels unlocked, tutorial shown, etc.)
    /// </summary>
    [ContextMenu("Reset All Progress")]
    public void ResetProgress()
    {
        Debug.Log("====================================");
        Debug.Log("🔄 RESETTING ALL PROGRESS");
        Debug.Log("====================================");
        
        // Reset level progress
        if (GameProgressManager.Instance != null)
        {
            GameProgressManager.Instance.ResetAllProgress();
            Debug.Log("✅ Level progress reset");
        }
        else
        {
            Debug.LogWarning("⚠️ GameProgressManager not found");
        }
        
        // Reset tutorial flags
        PlayerPrefs.DeleteKey("Tutorial_Shown");
        PlayerPrefs.DeleteKey("Level1_Tutorial_Shown");
        PlayerPrefs.DeleteKey("Level2_Tutorial_Shown");
        PlayerPrefs.DeleteKey("Level3_Tutorial_Shown");
        Debug.Log("✅ Tutorial flags reset");
        
        // Clear all PlayerPrefs (nuclear option)
        // PlayerPrefs.DeleteAll(); // Uncomment if you want to clear EVERYTHING
        
        PlayerPrefs.Save();
        
        Debug.Log("====================================");
        Debug.Log("✅ PROGRESS RESET COMPLETE!");
        Debug.Log("   - All levels locked except Level1");
        Debug.Log("   - Tutorial will show again");
        Debug.Log("====================================");
    }

    /// <summary>
    /// Show current progress information
    /// </summary>
    [ContextMenu("Show Progress Info")]
    public void ShowProgressInfo()
    {
        Debug.Log("====================================");
        Debug.Log("📊 CURRENT PROGRESS INFO");
        Debug.Log("====================================");
        
        if (GameProgressManager.Instance != null)
        {
            var allLevels = GameProgressManager.Instance.allLevels;
            Debug.Log($"📋 Total Levels: {allLevels.Count}");
            
            for (int i = 0; i < allLevels.Count; i++)
            {
                string level = allLevels[i];
                bool unlocked = GameProgressManager.Instance.IsLevelUnlocked(level);
                bool completed = GameProgressManager.Instance.IsLevelCompleted(level);
                
                string status = completed ? "✅ COMPLETED" : (unlocked ? "🔓 UNLOCKED" : "🔒 LOCKED");
                Debug.Log($"   [{i + 1}] {level}: {status}");
            }
            
            Debug.Log($"\n🎯 Next Level: {GameProgressManager.Instance.GetNextLevel(UnityEngine.SceneManagement.SceneManager.GetActiveScene().name) ?? "None (Final Level)"}");
        }
        else
        {
            Debug.LogError("❌ GameProgressManager not found!");
        }
        
        Debug.Log("====================================");
    }

    /// <summary>
    /// Unlock all levels (for testing)
    /// </summary>
    [ContextMenu("Unlock All Levels (Testing)")]
    public void UnlockAllLevels()
    {
        Debug.Log("🔓 Unlocking all levels for testing...");
        
        if (GameProgressManager.Instance != null)
        {
            var allLevels = GameProgressManager.Instance.allLevels;
            foreach (string level in allLevels)
            {
                GameProgressManager.Instance.MarkLevelComplete(level);
            }
            Debug.Log($"✅ All {allLevels.Count} levels unlocked!");
        }
        else
        {
            Debug.LogError("❌ GameProgressManager not found!");
        }
    }

    /// <summary>
    /// Complete current level (for testing)
    /// </summary>
    [ContextMenu("Complete Current Level")]
    public void CompleteCurrentLevel()
    {
        string currentScene = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;
        
        if (GameProgressManager.Instance != null)
        {
            GameProgressManager.Instance.MarkLevelComplete(currentScene);
            Debug.Log($"✅ Marked '{currentScene}' as completed");
        }
        else
        {
            Debug.LogError("❌ GameProgressManager not found!");
        }
    }
}
