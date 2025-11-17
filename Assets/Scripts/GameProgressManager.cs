using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using System.Linq;

/// <summary>
/// Persistent singleton that tracks player's level completion progress.
/// Saves/loads data using PlayerPrefs so progress persists across sessions.
/// 
/// AUTOMATIC LEVEL DETECTION:
/// - Automatically finds all scenes named "Level1", "Level2", "Level3", etc. from Build Settings
/// - Sorts them numerically (Level1, Level2, Level3, ...)
/// - Last level is automatically treated as the final level
/// - Just add new scenes named "LevelX" to Build Settings and they appear automatically!
/// 
/// Usage:
/// - Call MarkLevelComplete(sceneName) when player beats a level
/// - Call IsLevelUnlocked(sceneName) to check if a level is accessible
/// - Call GetUnlockedLevelCount() for UI display
/// - First level is always unlocked by default
/// </summary>
public class GameProgressManager : MonoBehaviour
{
    private static GameProgressManager _instance;
    public static GameProgressManager Instance
    {
        get
        {
            if (_instance == null)
            {
                // Try to find existing instance first (in case Awake already ran)
                _instance = FindAnyObjectByType<GameProgressManager>();
                
                if (_instance == null)
                {
                    Debug.Log("🎮 Creating new GameProgressManager instance...");
                    GameObject go = new GameObject("GameProgressManager");
                    _instance = go.AddComponent<GameProgressManager>();
                    DontDestroyOnLoad(go);
                    // Note: Awake() will handle AutoDetectLevels() and LoadProgress()
                }
            }
            return _instance;
        }
    }

    [Header("Level Configuration")]
    [Tooltip("Auto-detected from Build Settings. All scenes named 'Level1', 'Level2', etc. are automatically included.")]
    public List<string> allLevels = new List<string>();

    // Tracks which levels have been completed (persistent via PlayerPrefs)
    private HashSet<string> completedLevels = new HashSet<string>();

    // PlayerPrefs key for save data
    private const string PROGRESS_KEY = "GameProgress_CompletedLevels";

    private void Awake()
    {
        // Ensure singleton pattern - prevent duplicates
        if (_instance != null && _instance != this)
        {
            Debug.LogWarning($"⚠️ Duplicate GameProgressManager detected! Destroying duplicate.");
            Destroy(gameObject);
            return;
        }
        
        if (_instance == null)
        {
            _instance = this;
            DontDestroyOnLoad(gameObject);
            
            Debug.Log("🎮 GameProgressManager.Awake() - Initializing singleton...");
            
            // Auto-detect all Level scenes from Build Settings
            AutoDetectLevels();
            
            // Load saved progress
            LoadProgress();
        }
    }

    /// <summary>
    /// Automatically detects all scenes named "Level1", "Level2", "Level3", etc. from Build Settings.
    /// Sorts them numerically so they appear in correct order.
    /// </summary>
    private void AutoDetectLevels()
    {
        allLevels.Clear();
        
        // Get all scenes from Build Settings
        int sceneCount = SceneManager.sceneCountInBuildSettings;
        List<string> detectedLevels = new List<string>();
        
        for (int i = 0; i < sceneCount; i++)
        {
            string scenePath = SceneUtility.GetScenePathByBuildIndex(i);
            string sceneName = System.IO.Path.GetFileNameWithoutExtension(scenePath);
            
            // Check if scene name starts with "Level" followed by a number
            if (sceneName.StartsWith("Level") && sceneName.Length > 5)
            {
                string numberPart = sceneName.Substring(5); // Extract number after "Level"
                if (int.TryParse(numberPart, out int levelNumber))
                {
                    detectedLevels.Add(sceneName);
                }
            }
        }
        
        // Sort levels numerically (Level1, Level2, Level3, ...)
        allLevels = detectedLevels
            .OrderBy(name => int.Parse(name.Substring(5))) // Sort by number after "Level"
            .ToList();
        
        if (allLevels.Count > 0)
        {
            Debug.Log($"🎮 GameProgressManager: Auto-detected {allLevels.Count} levels:");
            for (int i = 0; i < allLevels.Count; i++)
            {
                string finalMarker = (i == allLevels.Count - 1) ? " (FINAL LEVEL)" : "";
                Debug.Log($"   [{i + 1}] {allLevels[i]}{finalMarker}");
            }
        }
        else
        {
            Debug.LogWarning("⚠️ GameProgressManager: No Level scenes found in Build Settings! Add scenes named 'Level1', 'Level2', etc.");
        }
    }

    /// <summary>
    /// Mark a level as completed and unlock the next level.
    /// Automatically saves progress to PlayerPrefs.
    /// </summary>
    public void MarkLevelComplete(string levelSceneName)
    {
        if (string.IsNullOrEmpty(levelSceneName))
        {
            Debug.LogWarning("GameProgressManager: Cannot mark empty level name as complete.");
            return;
        }

        if (!completedLevels.Contains(levelSceneName))
        {
            completedLevels.Add(levelSceneName);
            SaveProgress();
            Debug.Log($"✅ Level completed: {levelSceneName}. Progress saved.");
        }
    }

    /// <summary>
    /// Check if a level is unlocked and can be played.
    /// First level is always unlocked.
    /// Other levels unlock when the previous level is completed.
    /// </summary>
    public bool IsLevelUnlocked(string levelSceneName)
    {
        if (string.IsNullOrEmpty(levelSceneName)) return false;

        // Find index in level list
        int index = allLevels.IndexOf(levelSceneName);
        if (index == -1)
        {
            Debug.LogWarning($"GameProgressManager: Level '{levelSceneName}' not found in allLevels list.");
            return false; // Unknown level
        }

        // First level always unlocked
        if (index == 0)
        {
            Debug.Log($"🔓 {levelSceneName} is unlocked (first level, always available)");
            return true;
        }

        // Check if previous level is completed
        string previousLevel = allLevels[index - 1];
        bool isUnlocked = completedLevels.Contains(previousLevel);
        
        if (isUnlocked)
        {
            Debug.Log($"🔓 {levelSceneName} is unlocked (previous level '{previousLevel}' completed)");
        }
        else
        {
            Debug.Log($"🔒 {levelSceneName} is LOCKED (previous level '{previousLevel}' NOT completed)");
            Debug.Log($"   Current completed levels: {string.Join(", ", completedLevels)}");
        }
        
        return isUnlocked;
    }

    /// <summary>
    /// Check if a level has been completed (beaten).
    /// </summary>
    public bool IsLevelCompleted(string levelSceneName)
    {
        return completedLevels.Contains(levelSceneName);
    }

    /// <summary>
    /// Get the total number of unlocked levels (including completed ones).
    /// </summary>
    public int GetUnlockedLevelCount()
    {
        int count = 0;
        foreach (string level in allLevels)
        {
            if (IsLevelUnlocked(level)) count++;
            else break; // Sequential unlock: stop at first locked
        }
        return count;
    }

    /// <summary>
    /// Get the next level scene name after completing the current one.
    /// Returns empty string if there's no next level (final level beaten).
    /// </summary>
    public string GetNextLevel(string currentLevelSceneName)
    {
        int index = allLevels.IndexOf(currentLevelSceneName);
        if (index == -1 || index >= allLevels.Count - 1)
        {
            return ""; // No next level
        }
        return allLevels[index + 1];
    }

    /// <summary>
    /// Reset all progress (useful for testing or "New Game" button).
    /// </summary>
    public void ResetAllProgress()
    {
        completedLevels.Clear();
        PlayerPrefs.DeleteKey(PROGRESS_KEY);
        PlayerPrefs.Save();
        Debug.Log("🔄 All progress reset.");
    }

    // Save progress to PlayerPrefs (comma-separated list of completed level names)
    private void SaveProgress()
    {
        string data = string.Join(",", completedLevels);
        PlayerPrefs.SetString(PROGRESS_KEY, data);
        PlayerPrefs.Save();
        
        Debug.Log($"💾 SaveProgress() called. Saved data: '{data}'");
        Debug.Log($"💾 Completed levels count: {completedLevels.Count}");
        foreach (string level in completedLevels)
        {
            Debug.Log($"   ✅ Completed: {level}");
        }
    }

    // Load progress from PlayerPrefs
    private void LoadProgress()
    {
        completedLevels.Clear();
        string data = PlayerPrefs.GetString(PROGRESS_KEY, "");
        
        Debug.Log($"📂 LoadProgress() called. Raw PlayerPrefs data: '{data}'");
        
        if (!string.IsNullOrEmpty(data))
        {
            string[] levels = data.Split(',');
            foreach (string level in levels)
            {
                if (!string.IsNullOrEmpty(level))
                {
                    completedLevels.Add(level.Trim());
                    Debug.Log($"   ✅ Loaded completed level: {level.Trim()}");
                }
            }
            Debug.Log($"📂 Loaded progress: {completedLevels.Count} levels completed.");
        }
        else
        {
            Debug.Log("📂 No saved progress found (starting fresh).");
        }
    }
}
