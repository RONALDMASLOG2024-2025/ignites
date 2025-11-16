using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Displays a health bar above an enemy's head.
/// Automatically updates when the enemy takes damage.
/// 
/// Usage:
/// 1. Create a Canvas with "World Space" render mode
/// 2. Add an Image (background) and child Image (fill bar)
/// 3. Attach this script to the Canvas
/// 4. Assign the fill bar image reference
/// 5. Position above enemy sprite
/// </summary>
public class EnemyHealthBar : MonoBehaviour
{
    [Header("References")]
    [Tooltip("The fill bar image (usually green/red) that shrinks as health decreases")]
    public Image fillBar;
    
    [Tooltip("Reference to the enemy's health component (auto-finds if not set)")]
    public Enemy_Health enemyHealth;
    
    [Header("Visual Settings")]
    [Tooltip("Color when health is full")]
    public Color fullHealthColor = Color.green;
    
    [Tooltip("Color when health is low")]
    public Color lowHealthColor = Color.red;
    
    [Tooltip("Health percentage threshold for low health color (0-1)")]
    [Range(0f, 1f)]
    public float lowHealthThreshold = 0.3f;
    
    [Header("Behavior")]
    [Tooltip("Hide health bar when enemy is at full health")]
    public bool hideWhenFull = true;
    
    [Tooltip("Hide health bar when enemy dies")]
    public bool hideOnDeath = true;

    private Canvas canvas;
    private float maxHealth;
    private float currentHealth;

    private void Awake()
    {
        canvas = GetComponent<Canvas>();
        
        // Auto-find Enemy_Health component in parent
        if (enemyHealth == null)
        {
            enemyHealth = GetComponentInParent<Enemy_Health>();
            
            if (enemyHealth == null)
            {
                Debug.LogError("EnemyHealthBar: No Enemy_Health component found! Assign it manually or place this as child of enemy.");
            }
        }
    }

    private void Start()
    {
        if (enemyHealth != null)
        {
            maxHealth = enemyHealth.maxHealth;
            currentHealth = enemyHealth.currentHealth;
            
            // Hide initially if at full health
            if (hideWhenFull)
            {
                canvas.enabled = false;
            }
            
            UpdateHealthBar();
        }
    }

    private void Update()
    {
        if (enemyHealth != null)
        {
            // Check if health changed
            if (currentHealth != enemyHealth.currentHealth)
            {
                currentHealth = enemyHealth.currentHealth;
                UpdateHealthBar();
                
                // Show bar when damaged
                if (hideWhenFull && currentHealth < maxHealth)
                {
                    canvas.enabled = true;
                }
            }
            
            // Hide on death
            if (hideOnDeath && enemyHealth.currentHealth <= 0)
            {
                canvas.enabled = false;
            }
        }
    }

    private void UpdateHealthBar()
    {
        if (fillBar == null || maxHealth <= 0)
            return;
        
        // Calculate health percentage
        float healthPercent = Mathf.Clamp01(currentHealth / maxHealth);
        
        // Update fill amount
        fillBar.fillAmount = healthPercent;
        
        // Update color based on health
        if (healthPercent <= lowHealthThreshold)
        {
            fillBar.color = lowHealthColor;
        }
        else
        {
            // Smooth color transition from full to low health
            fillBar.color = Color.Lerp(lowHealthColor, fullHealthColor, 
                (healthPercent - lowHealthThreshold) / (1f - lowHealthThreshold));
        }
    }

    /// <summary>
    /// Manually update the health bar (call if needed)
    /// </summary>
    public void Refresh()
    {
        if (enemyHealth != null)
        {
            currentHealth = enemyHealth.currentHealth;
            UpdateHealthBar();
        }
    }
}
