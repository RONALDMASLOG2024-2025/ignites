using UnityEngine;
using UnityEngine.UI;
using TMPro;
#if UNITY_EDITOR
using UnityEditor;
#endif

/// <summary>
/// Helper script to validate Level Progression system setup.
/// Add this to any GameObject in MainMenu or gameplay scenes and click "Validate Setup" in Inspector.
/// </summary>
public class LevelProgressionValidator : MonoBehaviour
{
    [Header("Validation Results")]
    [TextArea(10, 20)]
    public string validationLog = "Click 'Validate Setup' button below to check configuration.";

    public void ValidateSetup()
    {
        validationLog = "=== LEVEL PROGRESSION VALIDATION ===\n\n";
        bool allGood = true;

        // Check GameProgressManager
        validationLog += "1. GameProgressManager:\n";
        if (GameProgressManager.Instance != null)
        {
            validationLog += "   ✅ Instance exists\n";
            var levels = GameProgressManager.Instance.allLevels;
            validationLog += $"   ✅ Configured levels: {levels.Count}\n";
            foreach (var level in levels)
            {
                validationLog += $"      - {level}\n";
            }
        }
        else
        {
            validationLog += "   ❌ GameProgressManager.Instance is NULL!\n";
            validationLog += "      → Will auto-create on first access\n";
        }

        // Check if we're in MainMenu scene
        if (UnityEngine.SceneManagement.SceneManager.GetActiveScene().name == "MainMenu")
        {
            validationLog += "\n2. MainMenu Scene Check:\n";
            
            // Check for LevelSelectUI
            LevelSelectUI levelSelect = FindFirstObjectByType<LevelSelectUI>();
            if (levelSelect != null)
            {
                validationLog += "   ✅ LevelSelectUI script found\n";
                
                if (levelSelect.buttonContainer != null)
                    validationLog += "   ✅ Button Container assigned\n";
                else
                {
                    validationLog += "   ❌ Button Container NOT assigned!\n";
                    allGood = false;
                }
                
                if (levelSelect.levelButtonPrefab != null)
                {
                    validationLog += "   ✅ Level Button Prefab assigned\n";
                    // Validate prefab has required components
                    if (levelSelect.levelButtonPrefab.GetComponent<Button>() != null)
                        validationLog += "      ✅ Prefab has Button component\n";
                    else
                    {
                        validationLog += "      ❌ Prefab missing Button component!\n";
                        allGood = false;
                    }
                    
                    if (levelSelect.levelButtonPrefab.GetComponentInChildren<TMP_Text>() != null)
                        validationLog += "      ✅ Prefab has TMP_Text component\n";
                    else
                    {
                        validationLog += "      ❌ Prefab missing TMP_Text component!\n";
                        allGood = false;
                    }
                }
                else
                {
                    validationLog += "   ❌ Level Button Prefab NOT assigned!\n";
                    allGood = false;
                }
            }
            else
            {
                validationLog += "   ❌ LevelSelectUI script NOT found in scene!\n";
                validationLog += "      → Add LevelSelectUI to a GameObject in MainMenu\n";
                allGood = false;
            }
        }
        else
        {
            // Check UIManager in gameplay scene
            validationLog += "\n2. Gameplay Scene Check:\n";
            string sceneName = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;
            validationLog += $"   Current Scene: {sceneName}\n";
            
            UIManager uiManager = FindFirstObjectByType<UIManager>();
            if (uiManager != null)
            {
                validationLog += "   ✅ UIManager found\n";
                
                if (!string.IsNullOrEmpty(uiManager.nextLevelSceneName))
                    validationLog += $"   ✅ Next Level: {uiManager.nextLevelSceneName}\n";
                else
                    validationLog += "   ⚠️  Next Level not set (OK if this is final level)\n";
                
                if (uiManager.victoryPanel != null)
                    validationLog += "   ✅ Victory Panel assigned\n";
                else
                {
                    validationLog += "   ❌ Victory Panel NOT assigned!\n";
                    allGood = false;
                }
                
                if (uiManager.gameOverPanel != null)
                    validationLog += "   ✅ Game Over Panel assigned\n";
                else
                {
                    validationLog += "   ❌ Game Over Panel NOT assigned!\n";
                    allGood = false;
                }
            }
            else
            {
                validationLog += "   ❌ UIManager NOT found in scene!\n";
                allGood = false;
            }
        }

        // Check MusicManager
        validationLog += "\n3. MusicManager:\n";
        MusicManager musicMgr = FindFirstObjectByType<MusicManager>();
        if (musicMgr != null)
        {
            validationLog += "   ✅ MusicManager found\n";
            if (musicMgr.normalMusic != null)
                validationLog += "   ✅ Normal Music assigned\n";
            else
                validationLog += "   ⚠️  Normal Music not assigned (no gameplay music)\n";
            
            if (musicMgr.menuMusic != null)
                validationLog += "   ✅ Menu Music assigned\n";
            else
                validationLog += "   ⚠️  Menu Music not assigned (no menu music)\n";
        }
        else
        {
            validationLog += "   ⚠️  MusicManager not found (will auto-create if exists in another scene)\n";
        }

        // Final summary
        validationLog += "\n=== SUMMARY ===\n";
        if (allGood)
        {
            validationLog += "✅ All critical components configured correctly!\n";
            validationLog += "   You should be able to:\n";
            validationLog += "   - See level buttons in MainMenu\n";
            validationLog += "   - Click unlocked levels to play\n";
            validationLog += "   - Progress to next level after victory\n";
        }
        else
        {
            validationLog += "❌ Some issues found! Fix the errors above.\n";
            validationLog += "   See Docs/URGENT_FIX_GUIDE.md for detailed setup instructions.\n";
        }

        Debug.Log(validationLog);
    }
}

#if UNITY_EDITOR
[CustomEditor(typeof(LevelProgressionValidator))]
public class LevelProgressionValidatorEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        
        LevelProgressionValidator validator = (LevelProgressionValidator)target;
        
        EditorGUILayout.Space();
        if (GUILayout.Button("Validate Setup", GUILayout.Height(40)))
        {
            validator.ValidateSetup();
        }
    }
}
#endif
