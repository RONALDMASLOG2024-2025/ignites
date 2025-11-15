using UnityEngine;

public class Enemy_Health : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public int currentHealth;
    public int maxHealth;

    [Header("Enemy Type")]
    [Tooltip("Check this if this enemy is a boss (won't count toward regular enemy total)")]
    public bool isBoss = false;

    [Header("Loot Drops")]
    [Tooltip("Prefab to spawn when enemy dies (e.g., meat pickup)")]
    public GameObject dropPrefab;
    
    [Tooltip("Chance to drop loot (0-100%). 100 = always drops")]
    [Range(0, 100)]
    public float dropChance = 50f;

    private void Start()
    {
        currentHealth = maxHealth;
        
        // Register with EnemyManager if this is a regular enemy
        if (!isBoss && EnemyManager.Instance != null)
        {
            EnemyManager.Instance.RegisterEnemy();
        }
    }

    public void ChangeHealth(int amount)
    {
        currentHealth += amount;

        if (currentHealth > maxHealth)
        {
            currentHealth = maxHealth;
        }
        else if (currentHealth <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        // Drop loot if configured
        if (dropPrefab != null && Random.Range(0f, 100f) <= dropChance)
        {
            Instantiate(dropPrefab, transform.position, Quaternion.identity);
            Debug.Log($"{name} dropped loot!");
        }

        // Notify manager if this is a regular enemy
        if (!isBoss && EnemyManager.Instance != null)
        {
            EnemyManager.Instance.UnregisterEnemy();
        }
        else if (isBoss && EnemyManager.Instance != null)
        {
            Debug.Log("Boss defeated!");
            EnemyManager.Instance.OnBossDefeated();
        }

        Destroy(gameObject);
    }

    private void OnDestroy()
    {
        // Safety: if the enemy is destroyed without calling Die() (e.g., via Destroy in another script),
        // still unregister. Check if we're being destroyed during play (not scene unload).
        if (!isBoss && EnemyManager.Instance != null && currentHealth > 0)
        {
            EnemyManager.Instance.UnregisterEnemy();
        }
    }
}
