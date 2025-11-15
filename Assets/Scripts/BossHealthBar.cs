using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Boss health bar UI component. Tracks and displays boss health.
/// Attach to a UI Canvas with a Slider component.
/// </summary>
public class BossHealthBar : MonoBehaviour
{
    [Header("UI Components")]
    public Slider healthSlider;
    public TMP_Text bossNameText;
    public TMP_Text healthText; // Optional: shows "100/200" format

    [Header("Boss Reference")]
    public Enemy_Health bossHealth;

    [Header("Settings")]
    public string bossName = "Boss";
    public bool showHealthNumbers = true;

    private GameObject uiPanel; // reference to parent panel to show/hide

    private void Start()
    {
        // Get the parent panel (usually the direct parent of this script's GameObject)
        uiPanel = gameObject;

        // Hide initially
        HideHealthBar();

        // Set boss name if text exists
        if (bossNameText != null)
        {
            bossNameText.text = bossName;
        }
    }

    private void Update()
    {
        if (bossHealth == null)
        {
            // Boss hasn't spawned yet or is dead
            HideHealthBar();
            return;
        }

        // Show the health bar
        if (!uiPanel.activeSelf)
        {
            ShowHealthBar();
        }

        // Update slider
        if (healthSlider != null)
        {
            healthSlider.maxValue = bossHealth.maxHealth;
            healthSlider.value = bossHealth.currentHealth;
        }

        // Update health text (optional)
        if (showHealthNumbers && healthText != null)
        {
            healthText.text = $"{bossHealth.currentHealth} / {bossHealth.maxHealth}";
        }

        // Hide when boss dies
        if (bossHealth.currentHealth <= 0)
        {
            Invoke(nameof(HideHealthBar), 1f); // delay to show empty bar briefly
        }
    }

    /// <summary>
    /// Call this when the boss spawns to link the health component and show the bar.
    /// </summary>
    public void SetBoss(Enemy_Health boss)
    {
        bossHealth = boss;
        ShowHealthBar();
    }

    public void ShowHealthBar()
    {
        if (uiPanel != null)
        {
            uiPanel.SetActive(true);
        }
    }

    public void HideHealthBar()
    {
        if (uiPanel != null)
        {
            uiPanel.SetActive(false);
        }
    }
}
