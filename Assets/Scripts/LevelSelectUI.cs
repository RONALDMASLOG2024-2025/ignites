using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
using System.Collections.Generic;

/// <summary>
/// Level selection UI for the Main Menu.
/// Dynamically generates buttons for all levels, showing unlock status.
/// Only unlocked levels are clickable.
/// 
/// Setup:
/// 1. Create a Canvas in MainMenu scene
/// 2. Add this script to a GameObject (e.g., "LevelSelectPanel")
/// 3. Create a GridLayoutGroup or VerticalLayoutGroup as parent for buttons
/// 4. Assign the container and button prefab in inspector
/// 5. Configure GameProgressManager.allLevels list with your level scene names
/// </summary>
public class LevelSelectUI : MonoBehaviour
{
    [Header("UI References")]
    [Tooltip("Container for level buttons (e.g., GridLayoutGroup or VerticalLayoutGroup)")]
    public Transform buttonContainer;
    
    [Tooltip("Prefab for level button. Should have Button, Image, and TMP_Text components.")]
    public GameObject levelButtonPrefab;

    [Header("Visual Styling")]
    public Color unlockedColor = Color.white;
    public Color lockedColor = new Color(0.5f, 0.5f, 0.5f, 0.5f); // Grayed out
    public string lockedText = "🔒 Locked";

    [Header("Optional: Progress Display")]
    public TMP_Text progressText; // e.g., "Progress: 2/5 Levels Completed"

    private void Start()
    {
        Debug.Log("LevelSelectUI: Starting initialization...");
        
        // Verify GameProgressManager exists
        if (GameProgressManager.Instance == null)
        {
            Debug.LogError("LevelSelectUI: GameProgressManager.Instance is NULL! Cannot generate level buttons.");
            return;
        }
        
        GenerateLevelButtons();
        UpdateProgressDisplay();
        
        Debug.Log("LevelSelectUI: Initialization complete.");
    }

    /// <summary>
    /// Creates a button for each level in GameProgressManager.allLevels.
    /// Locked levels are grayed out and non-interactable.
    /// </summary>
    private void GenerateLevelButtons()
    {
        Debug.Log("LevelSelectUI: GenerateLevelButtons() called.");
        
        if (buttonContainer == null)
        {
            Debug.LogError("LevelSelectUI: buttonContainer not assigned in inspector! Please assign a VerticalLayoutGroup or GridLayoutGroup.");
            return;
        }

        // If no prefab assigned, create buttons programmatically
        bool createDynamically = (levelButtonPrefab == null);
        if (createDynamically)
        {
            Debug.LogWarning("LevelSelectUI: levelButtonPrefab not assigned! Creating buttons dynamically (basic style).");
        }

        // Clear any existing buttons
        foreach (Transform child in buttonContainer)
        {
            Destroy(child.gameObject);
        }

        // Get level list from progress manager
        List<string> allLevels = GameProgressManager.Instance.allLevels;
        if (allLevels == null || allLevels.Count == 0)
        {
            Debug.LogWarning("LevelSelectUI: No levels configured in GameProgressManager.allLevels!");
            return;
        }

        Debug.Log($"LevelSelectUI: Generating {allLevels.Count} level buttons...");

        // Create button for each level
        for (int i = 0; i < allLevels.Count; i++)
        {
            string levelSceneName = allLevels[i];
            bool isUnlocked = GameProgressManager.Instance.IsLevelUnlocked(levelSceneName);
            bool isCompleted = GameProgressManager.Instance.IsLevelCompleted(levelSceneName);

            Debug.Log($"LevelSelectUI: Creating button for {levelSceneName} - Unlocked: {isUnlocked}, Completed: {isCompleted}");

            GameObject buttonObj;
            
            if (createDynamically)
            {
                // Create button programmatically
                buttonObj = CreateButtonDynamically(levelSceneName, i + 1, isUnlocked, isCompleted);
            }
            else
            {
                // Use prefab
                buttonObj = Instantiate(levelButtonPrefab, buttonContainer);
                buttonObj.name = $"LevelButton_{i + 1}";

                // Get components
                Button button = buttonObj.GetComponent<Button>();
                TMP_Text buttonText = buttonObj.GetComponentInChildren<TMP_Text>();
                Image buttonImage = buttonObj.GetComponent<Image>();

                if (button == null || buttonText == null)
                {
                    Debug.LogError($"LevelSelectUI: Button prefab missing Button or TMP_Text component!");
                    continue;
                }

                // Configure button
                ConfigureButton(button, buttonText, buttonImage, levelSceneName, i + 1, isUnlocked, isCompleted, buttonObj.transform);
            }
        }
        
        Debug.Log($"LevelSelectUI: Successfully created {allLevels.Count} level buttons!");
    }

    /// <summary>
    /// Create a button dynamically without a prefab (fallback).
    /// </summary>
    private GameObject CreateButtonDynamically(string levelSceneName, int levelNumber, bool isUnlocked, bool isCompleted)
    {
        // Create button GameObject
        GameObject buttonObj = new GameObject($"LevelButton_{levelNumber}");
        buttonObj.transform.SetParent(buttonContainer, false);
        
        // Add RectTransform
        RectTransform rectTransform = buttonObj.AddComponent<RectTransform>();
        rectTransform.sizeDelta = new Vector2(300, 80);
        
        // Add Image (background)
        Image buttonImage = buttonObj.AddComponent<Image>();
        buttonImage.color = isUnlocked ? unlockedColor : lockedColor;
        
        // Add Button component
        Button button = buttonObj.AddComponent<Button>();
        button.targetGraphic = buttonImage;
        
        // Create text child
        GameObject textObj = new GameObject("Text");
        textObj.transform.SetParent(buttonObj.transform, false);
        
        RectTransform textRect = textObj.AddComponent<RectTransform>();
        textRect.anchorMin = Vector2.zero;
        textRect.anchorMax = Vector2.one;
        textRect.sizeDelta = Vector2.zero;
        
        TMP_Text buttonText = textObj.AddComponent<TextMeshProUGUI>();
        buttonText.alignment = TextAlignmentOptions.Center;
        buttonText.fontSize = 28;
        buttonText.color = Color.black;
        
        // Configure button
        ConfigureButton(button, buttonText, buttonImage, levelSceneName, levelNumber, isUnlocked, isCompleted, buttonObj.transform);
        
        return buttonObj;
    }

    /// <summary>
    /// Configure button properties (works for both prefab and dynamic buttons).
    /// </summary>
    private void ConfigureButton(Button button, TMP_Text buttonText, Image buttonImage, 
                                   string levelSceneName, int levelNumber, bool isUnlocked, 
                                   bool isCompleted, Transform buttonTransform)
    {
        if (isUnlocked)
        {
            // Unlocked: clickable and normal color
            buttonText.text = GetLevelDisplayName(levelSceneName, levelNumber, isCompleted);
            if (buttonImage != null) buttonImage.color = unlockedColor;
            button.interactable = true;

            // Hide lock icon if it exists
            Transform lockIcon = buttonTransform.Find("LockedIcon");
            if (lockIcon != null) lockIcon.gameObject.SetActive(false);

            // Show/hide star icon based on completion
            Transform starIcon = buttonTransform.Find("CompletedStar");
            if (starIcon != null) starIcon.gameObject.SetActive(isCompleted);

            // Add click listener
            string sceneName = levelSceneName; // Capture for closure
            button.onClick.AddListener(() => LoadLevel(sceneName));
        }
        else
        {
            // Locked: grayed out and non-interactable
            buttonText.text = lockedText;
            if (buttonImage != null) buttonImage.color = lockedColor;
            button.interactable = false;

            // Show lock icon if it exists
            Transform lockIcon = buttonTransform.Find("LockedIcon");
            if (lockIcon != null) lockIcon.gameObject.SetActive(true);

            // Hide star icon (locked levels can't be completed)
            Transform starIcon = buttonTransform.Find("CompletedStar");
            if (starIcon != null) starIcon.gameObject.SetActive(false);
        }
    }

    /// <summary>
    /// Get display name for a level button.
    /// Override this method to customize level names (e.g., read from ScriptableObject).
    /// </summary>
    private string GetLevelDisplayName(string sceneName, int levelNumber, bool isCompleted)
    {
        string completedMarker = isCompleted ? "" : "";
        
        // You can customize level names here
        // For now, just use "Level 1", "Level 2", etc.
        return $"Level {levelNumber}{completedMarker}";
    }

    /// <summary>
    /// Load the selected level scene.
    /// </summary>
    private void LoadLevel(string sceneName)
    {
        Debug.Log($"🎮 Loading level: {sceneName}");
        
        // Reset game state before loading
        if (GameState.Instance != null)
        {
            GameState.Instance.ResetState();
        }

        SceneManager.LoadScene(sceneName);
    }

    /// <summary>
    /// Update progress text display (e.g., "Progress: 2/5 Levels Completed").
    /// </summary>
    private void UpdateProgressDisplay()
    {
        if (progressText == null) return;

        List<string> allLevels = GameProgressManager.Instance.allLevels;
        int totalLevels = allLevels.Count;
        int completedCount = 0;

        foreach (string level in allLevels)
        {
            if (GameProgressManager.Instance.IsLevelCompleted(level))
            {
                completedCount++;
            }
        }

        progressText.text = $"Progress: {completedCount}/{totalLevels} Levels Completed";
    }

    /// <summary>
    /// Refresh the level select UI (call this after unlocking new levels).
    /// </summary>
    public void RefreshUI()
    {
        GenerateLevelButtons();
        UpdateProgressDisplay();
    }

    /// <summary>
    /// Reset all progress (for "New Game" or "Reset Progress" button).
    /// WARNING: This will erase all saved progress!
    /// </summary>
    public void OnResetProgressButton()
    {
        // Show confirmation dialog in production!
        Debug.LogWarning("⚠️ Resetting all progress...");
        GameProgressManager.Instance.ResetAllProgress();
        RefreshUI();
    }
}
