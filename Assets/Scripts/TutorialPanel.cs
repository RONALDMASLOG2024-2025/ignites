using UnityEngine;
using UnityEngine.UI;
using System.Collections;

/// <summary>
/// Displays tutorial instructions at the start of a level, then fades out automatically.
/// Attach to a tutorial panel UI GameObject.
/// 
/// Usage:
/// 1. Create a UI Panel with Image (semi-transparent background)
/// 2. Add Text/TextMeshPro elements showing controls
/// 3. Attach this script to the panel
/// 4. Configure display duration and fade speed
/// </summary>
public class TutorialPanel : MonoBehaviour
{
    [Header("Display Settings")]
    [Tooltip("How long the tutorial stays visible before fading (seconds)")]
    public float displayDuration = 5f;
    
    [Tooltip("How long the fade-out animation takes (seconds)")]
    public float fadeDuration = 1f;
    
    [Tooltip("If true, player can press any key to skip the tutorial")]
    public bool allowSkip = true;
    
    [Tooltip("Block pause (ESC) while tutorial is showing")]
    public bool blockPauseDuringTutorial = true;
    
    [Header("References")]
    [Tooltip("The CanvasGroup component for fading (will auto-find if not set)")]
    public CanvasGroup canvasGroup;
    
    [Tooltip("Reference to UIManager to control HUD visibility (auto-finds if not set)")]
    public UIManager uiManager;
    
    [Header("Optional")]
    [Tooltip("Show tutorial only on first playthrough of this level")]
    public bool showOnlyOnce = true;
    
    [Tooltip("PlayerPrefs key to track if tutorial was shown")]
    public string tutorialKey = "Tutorial_Shown";

    private bool isFading = false;
    private bool hasBeenShown = false;
    private bool hudWasHidden = false;
    private bool isTutorialActive = false;

    private void Awake()
    {
        // Auto-find CanvasGroup if not assigned
        if (canvasGroup == null)
        {
            canvasGroup = GetComponent<CanvasGroup>();
            if (canvasGroup == null)
            {
                canvasGroup = gameObject.AddComponent<CanvasGroup>();
                Debug.Log("TutorialPanel: Added CanvasGroup component automatically");
            }
        }
        
        // Auto-find UIManager if not assigned
        if (uiManager == null)
        {
            uiManager = Object.FindAnyObjectByType<UIManager>();
            if (uiManager == null)
            {
                Debug.LogWarning("TutorialPanel: No UIManager found. HUD will not be hidden during tutorial.");
            }
        }
    }

    private void Start()
    {
        // Check if tutorial should be shown
        if (showOnlyOnce)
        {
            hasBeenShown = PlayerPrefs.GetInt(tutorialKey, 0) == 1;
            
            if (hasBeenShown)
            {
                // Hide immediately if already shown before
                gameObject.SetActive(false);
                // Make sure HUD is visible if tutorial is skipped
                ShowHUD();
                return;
            }
        }

        // Show tutorial
        canvasGroup.alpha = 1f;
        gameObject.SetActive(true);
        isTutorialActive = true;
        
        // Hide HUD while tutorial is showing (use coroutine for better timing)
        StartCoroutine(HideHUDAfterFrame());
        
        // Start auto-fade timer
        StartCoroutine(AutoFadeRoutine());
    }
    
    /// <summary>
    /// Waits a frame to ensure UIManager has initialized, then hides HUD
    /// </summary>
    private IEnumerator HideHUDAfterFrame()
    {
        yield return new WaitForEndOfFrame();
        HideHUD();
    }

    private void Update()
    {
        // Prevent pause during tutorial
        if (isTutorialActive && blockPauseDuringTutorial && Input.GetKeyDown(KeyCode.Escape))
        {
            // Skip tutorial with ESC instead of pausing
            if (!isFading)
            {
                Debug.Log("Tutorial skipped with ESC");
                StartCoroutine(FadeOut());
            }
            return; // Don't process other keys
        }
        
        // Allow skip with any key press (except ESC if we're handling it above)
        if (allowSkip && !isFading && isTutorialActive)
        {
            // Check for any key except ESC (ESC is handled separately above)
            if (Input.anyKeyDown && !Input.GetKeyDown(KeyCode.Escape))
            {
                Debug.Log("Tutorial skipped by player");
                StartCoroutine(FadeOut());
            }
        }
    }

    private IEnumerator AutoFadeRoutine()
    {
        // Wait for display duration
        yield return new WaitForSeconds(displayDuration);
        
        // Then fade out
        if (!isFading)
        {
            StartCoroutine(FadeOut());
        }
    }

    private IEnumerator FadeOut()
    {
        isFading = true;
        float elapsed = 0f;
        float startAlpha = canvasGroup.alpha;

        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            canvasGroup.alpha = Mathf.Lerp(startAlpha, 0f, elapsed / fadeDuration);
            yield return null;
        }

        canvasGroup.alpha = 0f;
        
        // Mark as shown
        if (showOnlyOnce)
        {
            PlayerPrefs.SetInt(tutorialKey, 1);
            PlayerPrefs.Save();
        }
        
        // Tutorial is no longer active
        isTutorialActive = false;
        
        // Show HUD when tutorial is done
        ShowHUD();
        
        gameObject.SetActive(false);
        Debug.Log("Tutorial panel hidden - HUD restored");
    }

    /// <summary>
    /// Call this to manually show the tutorial again (e.g., from help button)
    /// </summary>
    public void ShowTutorial()
    {
        StopAllCoroutines();
        isFading = false;
        isTutorialActive = true;
        gameObject.SetActive(true);
        canvasGroup.alpha = 1f;
        
        // Hide HUD when showing tutorial
        HideHUD();
        
        StartCoroutine(AutoFadeRoutine());
    }

    /// <summary>
    /// Reset the "shown" flag for testing
    /// </summary>
    public void ResetTutorialFlag()
    {
        PlayerPrefs.DeleteKey(tutorialKey);
        Debug.Log("Tutorial flag reset - will show again on next level load");
    }
    
    /// <summary>
    /// Check if tutorial is currently active (for external scripts to query)
    /// </summary>
    public bool IsTutorialActive()
    {
        return isTutorialActive && gameObject.activeInHierarchy;
    }
    
    /// <summary>
    /// Hides the HUD panel via UIManager
    /// </summary>
    private void HideHUD()
    {
        if (uiManager != null && uiManager.hudPanel != null)
        {
            uiManager.hudPanel.SetActive(false);
            hudWasHidden = true;
            Debug.Log("TutorialPanel: HUD hidden");
        }
        else
        {
            Debug.LogWarning("TutorialPanel: Cannot hide HUD - UIManager or hudPanel is null");
        }
    }
    
    /// <summary>
    /// Shows the HUD panel via UIManager
    /// </summary>
    private void ShowHUD()
    {
        if (hudWasHidden && uiManager != null && uiManager.hudPanel != null)
        {
            uiManager.hudPanel.SetActive(true);
            hudWasHidden = false;
            Debug.Log("TutorialPanel: HUD restored");
        }
    }
}
