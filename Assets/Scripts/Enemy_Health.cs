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
    [Tooltip("Meat drop - heals player")]
    public GameObject meatDropPrefab;
    [Range(0, 100)]
    public float meatDropChance = 50f;
    
    [Tooltip("Bone drop - enhances player stats (damage/speed)")]
    public GameObject boneDropPrefab;
    [Range(0, 100)]
    public float boneDropChance = 30f;
    
    [Header("Legacy Drop (deprecated - use specific drops above)")]
    [Tooltip("Old drop system - will be removed. Use meatDropPrefab/boneDropPrefab instead")]
    public GameObject dropPrefab;
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
        // Drop meat if configured
        if (meatDropPrefab != null && Random.Range(0f, 100f) <= meatDropChance)
        {
            Instantiate(meatDropPrefab, transform.position, Quaternion.identity);
            Debug.Log($"{name} dropped meat!");
        }
        
        // Drop bone if configured (independent roll)
        if (boneDropPrefab != null && Random.Range(0f, 100f) <= boneDropChance)
        {
            Vector3 bonePosition = transform.position + new Vector3(Random.Range(-0.5f, 0.5f), 0, 0);
            Instantiate(boneDropPrefab, bonePosition, Quaternion.identity);
            Debug.Log($"{name} dropped bone!");
        }
        
        // Legacy drop system (for backwards compatibility)
        if (dropPrefab != null && Random.Range(0f, 100f) <= dropChance)
        {
            Instantiate(dropPrefab, transform.position, Quaternion.identity);
            Debug.Log($"{name} dropped loot (legacy)!");
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
