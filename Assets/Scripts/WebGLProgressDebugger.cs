using UnityEngine;

/// <summary>
/// Special debugger for WebGL builds to diagnose PlayerPrefs/localStorage issues.
/// Attach to any GameObject or use context menu in Inspector.
/// </summary>
public class WebGLProgressDebugger : MonoBehaviour
{
    [Header("Debug Options")]
    [Tooltip("Show detailed localStorage info on Start")]
    public bool logOnStart = true;
    
    [Header("Manual Triggers")]
    [Tooltip("Check this to dump all PlayerPrefs data")]
    public bool dumpPlayerPrefs = false;
    
    [Tooltip("Check this to verify localStorage persistence")]
    public bool testLocalStorage = false;

    private void Start()
    {
        if (logOnStart)
        {
            LogProgressState();
        }
    }

    private void Update()
    {
        if (dumpPlayerPrefs)
        {
            dumpPlayerPrefs = false;
            DumpAllPlayerPrefs();
        }
        
        if (testLocalStorage)
        {
            testLocalStorage = false;
            TestLocalStoragePersistence();
        }
    }

    [ContextMenu("Log Progress State")]
    public void LogProgressState()
    {
        Debug.Log("=== WebGL Progress Debugger ===");
        Debug.Log($"Platform: {Application.platform}");
        Debug.Log($"Is WebGL: {Application.platform == RuntimePlatform.WebGLPlayer}");
        
        if (GameProgressManager.Instance != null)
        {
            Debug.Log($"GameProgressManager Instance: EXISTS");
            Debug.Log($"All Levels: {string.Join(", ", GameProgressManager.Instance.allLevels)}");
            
            string progressData = PlayerPrefs.GetString("GameProgress_CompletedLevels", "");
            Debug.Log($"Raw PlayerPrefs data: '{progressData}'");
            
            for (int i = 0; i < GameProgressManager.Instance.allLevels.Count; i++)
            {
                string level = GameProgressManager.Instance.allLevels[i];
                bool unlocked = GameProgressManager.Instance.IsLevelUnlocked(level);
                bool completed = GameProgressManager.Instance.IsLevelCompleted(level);
                Debug.Log($"  [{i}] {level}: Unlocked={unlocked}, Completed={completed}");
            }
        }
        else
        {
            Debug.LogError("GameProgressManager Instance: NULL!");
        }
        
#if UNITY_WEBGL && !UNITY_EDITOR
        Debug.Log("💾 WebGL localStorage status:");
        Debug.Log("   autoSyncPersistentDataPath should be enabled in index.html");
        Debug.Log("   Check browser console for IndexedDB errors");
#endif
    }

    [ContextMenu("Dump All PlayerPrefs")]
    public void DumpAllPlayerPrefs()
    {
        Debug.Log("=== All PlayerPrefs Keys ===");
        
        // Known keys in this game
        string[] knownKeys = new string[]
        {
            "GameProgress_CompletedLevels",
            "Tutorial_Shown",
            "Highscore",
            "MusicVolume",
            "SFXVolume"
        };
        
        foreach (string key in knownKeys)
        {
            if (PlayerPrefs.HasKey(key))
            {
                string value = PlayerPrefs.GetString(key, "N/A");
                int intValue = PlayerPrefs.GetInt(key, -999);
                float floatValue = PlayerPrefs.GetFloat(key, -999f);
                
                Debug.Log($"  {key}:");
                Debug.Log($"    String: '{value}'");
                Debug.Log($"    Int: {intValue}");
                Debug.Log($"    Float: {floatValue}");
            }
            else
            {
                Debug.Log($"  {key}: NOT FOUND");
            }
        }
        
        Debug.Log("=== End PlayerPrefs Dump ===");
    }

    [ContextMenu("Test LocalStorage Persistence")]
    public void TestLocalStoragePersistence()
    {
#if UNITY_WEBGL && !UNITY_EDITOR
        Debug.Log("🧪 Testing WebGL localStorage persistence...");
        
        string testKey = "WebGLTest_" + System.DateTime.Now.Ticks;
        string testValue = "TestValue_" + System.DateTime.Now.Ticks;
        
        // Write test value
        PlayerPrefs.SetString(testKey, testValue);
        PlayerPrefs.Save();
        Debug.Log($"   ✍️ Wrote test key: {testKey} = {testValue}");
        
        // Read back immediately
        string readValue = PlayerPrefs.GetString(testKey, "FAILED");
        if (readValue == testValue)
        {
            Debug.Log($"   ✅ Immediate read SUCCESS: {readValue}");
        }
        else
        {
            Debug.LogError($"   ❌ Immediate read FAILED: Expected '{testValue}', got '{readValue}'");
        }
        
        // Clean up
        PlayerPrefs.DeleteKey(testKey);
        PlayerPrefs.Save();
        Debug.Log("   🧹 Cleaned up test key");
#else
        Debug.Log("Test only runs in WebGL builds (not Editor)");
#endif
    }

    [ContextMenu("Force Save Current Progress")]
    public void ForceSaveProgress()
    {
        if (GameProgressManager.Instance != null)
        {
            // Trigger a manual save by marking current level complete (if in a level)
            string currentScene = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;
            if (currentScene.StartsWith("Level"))
            {
                GameProgressManager.Instance.MarkLevelComplete(currentScene);
                Debug.Log($"✅ Force-saved progress for {currentScene}");
            }
            else
            {
                Debug.LogWarning("Not in a level scene, can't force-save progress");
            }
        }
        
        PlayerPrefs.Save();
        Debug.Log("💾 Called PlayerPrefs.Save()");
    }
}
