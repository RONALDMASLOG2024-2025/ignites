using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Quick helper to verify MainMenu LevelSelectPanel is configured correctly.
/// Add this to LevelSelectPanel GameObject and press Play to auto-check.
/// </summary>
public class LevelSelectQuickFix : MonoBehaviour
{
    private void Start()
    {
        Debug.Log("=== LEVEL SELECT QUICK FIX CHECK ===");
        
        // Check if LevelSelectUI script exists
        LevelSelectUI levelSelectUI = GetComponent<LevelSelectUI>();
        if (levelSelectUI == null)
        {
            Debug.LogError("❌ LevelSelectUI script missing! Add it to this GameObject.");
            return;
        }
        
        Debug.Log("✅ LevelSelectUI script found");
        
        // Check ButtonContainer
        if (levelSelectUI.buttonContainer == null)
        {
            Debug.LogWarning("⚠️  ButtonContainer not assigned! Looking for it...");
            
            // Try to find VerticalLayoutGroup child
            VerticalLayoutGroup vlg = GetComponentInChildren<VerticalLayoutGroup>();
            if (vlg != null)
            {
                Debug.Log($"✅ Found VerticalLayoutGroup: {vlg.name}");
                Debug.Log("   Assign it to Button Container field in Inspector!");
            }
            else
            {
                Debug.LogError("❌ No VerticalLayoutGroup found in children!");
                Debug.LogError("   Create one: Right-click LevelSelectPanel → UI → Vertical Layout Group");
            }
        }
        else
        {
            Debug.Log($"✅ ButtonContainer assigned: {levelSelectUI.buttonContainer.name}");
        }
        
        // Check prefab (optional now)
        if (levelSelectUI.levelButtonPrefab == null)
        {
            Debug.LogWarning("⚠️  Level Button Prefab not assigned (will create buttons dynamically)");
        }
        else
        {
            Debug.Log($"✅ Button Prefab assigned: {levelSelectUI.levelButtonPrefab.name}");
        }
        
        // Check GameProgressManager
        if (GameProgressManager.Instance != null)
        {
            Debug.Log($"✅ GameProgressManager found with {GameProgressManager.Instance.allLevels.Count} levels");
            foreach (var level in GameProgressManager.Instance.allLevels)
            {
                bool unlocked = GameProgressManager.Instance.IsLevelUnlocked(level);
                Debug.Log($"   - {level}: {(unlocked ? "Unlocked ✓" : "Locked 🔒")}");
            }
        }
        else
        {
            Debug.LogError("❌ GameProgressManager.Instance is NULL!");
        }
        
        Debug.Log("=== END CHECK ===");
        Debug.Log("If you see errors above, fix them and press Play again.");
        
        // Auto-destroy this helper after checking
        Destroy(this, 1f);
    }
}
