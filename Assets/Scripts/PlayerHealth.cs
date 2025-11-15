using Unity.Mathematics;
using UnityEngine;
using TMPro;

public class PlayerHealth : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public int currentHealth;
    public int maxHealth;

    public Animator TextAnimation;

    public TMP_Text healthText;
    // Flag to mark that the player has died. Keep the GameObject active to avoid
    // "Coroutine couldn't be started because the game object is inactive" errors.
    public bool isDead = false;

    private void Start()
    {
        // Reset to full health at scene start (each level starts fresh)
        ResetHealth();
        healthText.text = "Life: " + currentHealth + "/" + maxHealth;
        TextAnimation.Play("LifeTextAnim");
    }

    /// <summary>
    /// Resets player to full health and clears dead flag.
    /// Called at scene start so each level begins with fresh stats.
    /// </summary>
    public void ResetHealth()
    {
        currentHealth = maxHealth;
        isDead = false;
        
        // Re-enable movement and combat if they were disabled
        var pm = GetComponent<PlayerMovement>();
        if (pm != null) pm.enabled = true;
        var pc = GetComponent<Player_Combat>();
        if (pc != null) pc.enabled = true;
        
        Debug.Log($"Player health reset to {maxHealth} (fresh level start)");
    }

    public void UpdateHealth(int amount)
    {
        // Apply change and clamp so health never exceeds max or drops below 0
        currentHealth += amount;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);

        // Update UI using the clamped value
        if (healthText != null)
            healthText.text = "Life: " + currentHealth + "/" + maxHealth;

        if (TextAnimation != null)
            TextAnimation.Play("LifeTextAnim");

        Debug.Log("Player Life: " + currentHealth);

        if (currentHealth <= 0)
        {
            Debug.Log("Player has died.");
            // Mark dead instead of deactivating the GameObject immediately.
            // Deactivating causes other scripts (that may still be running in the same
            // frame) to try to StartCoroutine on this MonoBehaviour and throw the
            // "game object is inactive" error. Instead, disable player control
            // components so the player stops interacting while keeping the GameObject
            // active so coroutines and other cleanup code can run safely.
            isDead = true;

            // Disable movement and combat scripts if present
            var pm = GetComponent<PlayerMovement>();
            if (pm != null) pm.enabled = false;
            var pc = GetComponent<Player_Combat>();
            if (pc != null) pc.enabled = false;
            // Show Game Over UI if available
            UIManager ui = Object.FindAnyObjectByType<UIManager>();
            if (ui != null)
            {
                ui.ShowGameOver();
            }
            
            // Notify centralized GameState of game over (broadcasts to all subscribers including MusicManager)
            if (GameState.Instance != null)
            {
                GameState.Instance.SetGameOver();
            }

            // Optionally keep the GameObject active for a short time so any
            // incoming calls (knockback, animations, coroutines) won't error.
            // If you want to hide the player after death, disable the renderer or
            // play a death animation then deactivate in a coroutine.
        }

    }
}
