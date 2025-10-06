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

    private void Start()
    {
        healthText.text = "Life: " + currentHealth + "/" + maxHealth;
        TextAnimation.Play("LifeTextAnim");
    }

    public void UpdateHealth(int amount)
    {
        currentHealth += amount;
        healthText.text = "Life: " + currentHealth + "/" + maxHealth;
        TextAnimation.Play("LifeTextAnim");
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
        Debug.Log("Player Life: " + currentHealth);

        if (currentHealth <= 0)
        {
            Debug.Log("Player has died.");
            gameObject.SetActive(false);
            // Add death handling logic here (e.g., respawn, game over screen)
        }

    }
}
