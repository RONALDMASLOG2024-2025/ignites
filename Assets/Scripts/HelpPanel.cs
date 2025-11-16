using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Manages the Help/Instructions panel accessible from the pause menu.
/// Shows detailed game controls and mechanics.
/// Attach to the help panel UI GameObject.
/// 
/// Usage:
/// 1. Create a UI Panel with scrollable content
/// 2. Add sections for Controls, Combat, Tips, etc.
/// 3. Attach this script to the panel
/// 4. Reference it from UIManager or pause menu
/// </summary>
public class HelpPanel : MonoBehaviour
{
    [Header("References")]
    [Tooltip("Reference to the pause menu panel (to hide/show when toggling help)")]
    public GameObject pauseMenuPanel;
    
    [Header("Optional Audio")]
    public AudioSource audioSource;
    public AudioClip openSound;
    public AudioClip closeSound;

    private bool isShowing = false;

    private void Start()
    {
        // Hide help panel by default
        gameObject.SetActive(false);
    }

    /// <summary>
    /// Shows the help panel (called from Help button in pause menu)
    /// </summary>
    public void ShowHelp()
    {
        gameObject.SetActive(true);
        isShowing = true;
        
        // Hide pause menu when showing help
        if (pauseMenuPanel != null)
        {
            pauseMenuPanel.SetActive(false);
        }

        // Play open sound
        if (audioSource != null && openSound != null)
        {
            audioSource.PlayOneShot(openSound);
        }

        Debug.Log("Help panel opened");
    }

    /// <summary>
    /// Hides the help panel and returns to pause menu
    /// </summary>
    public void HideHelp()
    {
        gameObject.SetActive(false);
        isShowing = false;
        
        // Show pause menu again
        if (pauseMenuPanel != null)
        {
            pauseMenuPanel.SetActive(true);
        }

        // Play close sound
        if (audioSource != null && closeSound != null)
        {
            audioSource.PlayOneShot(closeSound);
        }

        Debug.Log("Help panel closed");
    }

    /// <summary>
    /// Toggles help panel visibility
    /// </summary>
    public void ToggleHelp()
    {
        if (isShowing)
        {
            HideHelp();
        }
        else
        {
            ShowHelp();
        }
    }

    /// <summary>
    /// Back button - returns to pause menu
    /// </summary>
    public void OnBackButton()
    {
        HideHelp();
    }
}
