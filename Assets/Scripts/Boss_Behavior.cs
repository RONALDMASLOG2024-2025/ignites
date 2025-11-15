using System.Collections;
using UnityEngine;

/// <summary>
/// Boss-specific behavior: phases, minion summoning, and enhanced AI.
/// Attach this to your boss prefab alongside Enemy_Health, Enemy_Movement, etc.
/// </summary>
public class Boss_Behavior : MonoBehaviour
{
    [Header("Phase System")]
    [Tooltip("Current phase (1, 2, or 3)")]
    public int currentPhase = 1;
    
    [Tooltip("Health % to trigger Phase 2 (e.g., 50 = 50% health)")]
    public float phase2Threshold = 50f;
    
    [Tooltip("Health % to trigger Phase 3 (e.g., 25 = 25% health)")]
    public float phase3Threshold = 25f;

    [Header("Phase 2 Bonuses")]
    public float phase2SpeedMultiplier = 1.5f;
    public Color phase2Color = Color.yellow;

    [Header("Phase 3 Bonuses")]
    public float phase3SpeedMultiplier = 2f;
    public Color phase3Color = Color.red;
    public float phase3AttackCooldownMultiplier = 0.7f; // faster attacks

    [Header("Minion Summoning")]
    public GameObject minionPrefab;
    public int minionsPerSummon = 2;
    public float summonCooldown = 15f; // seconds between summons
    
    private float nextSummonTime = 0f;
    private bool hasEnteredPhase2 = false;
    private bool hasEnteredPhase3 = false;

    // Component references
    private Enemy_Health health;
    private Enemy_Movement movement;
    private SpriteRenderer spriteRenderer;

    private void Start()
    {
        health = GetComponent<Enemy_Health>();
        movement = GetComponent<Enemy_Movement>();
        spriteRenderer = GetComponent<SpriteRenderer>();

        if (health == null)
        {
            Debug.LogError("Boss_Behavior: Missing Enemy_Health component!");
        }

        // Set initial summon time
        nextSummonTime = Time.time + summonCooldown;
    }

    private void Update()
    {
        // Always respect centralized GameState for pause/game-end when available
        if (GameState.Instance != null && GameState.Instance.IsPaused)
        {
            return;
        }

        if (health == null) return;        // Check for phase transitions based on health percentage
        float healthPercent = (float)health.currentHealth / health.maxHealth * 100f;

        if (!hasEnteredPhase2 && healthPercent <= phase2Threshold)
        {
            EnterPhase2();
        }
        else if (!hasEnteredPhase3 && healthPercent <= phase3Threshold)
        {
            EnterPhase3();
        }

        // Auto-summon minions on cooldown (Phase 2 and 3 only)
        if (currentPhase >= 2 && Time.time >= nextSummonTime && minionPrefab != null)
        {
            SummonMinions();
            nextSummonTime = Time.time + summonCooldown;
        }
    }

    private void EnterPhase2()
    {
        hasEnteredPhase2 = true;
        currentPhase = 2;
        Debug.Log("🔥 BOSS ENTERED PHASE 2!");

        // Increase movement speed
        if (movement != null)
        {
            movement.speed *= phase2SpeedMultiplier;
        }

        // Change sprite color to yellow
        if (spriteRenderer != null)
        {
            spriteRenderer.color = phase2Color;
        }

        // Summon first wave of minions immediately
        if (minionPrefab != null)
        {
            SummonMinions();
            nextSummonTime = Time.time + summonCooldown;
        }
    }

    private void EnterPhase3()
    {
        hasEnteredPhase3 = true;
        currentPhase = 3;
        Debug.Log("⚡ BOSS ENTERED PHASE 3 - ENRAGED!");

        // Further increase speed
        if (movement != null)
        {
            movement.speed *= (phase3SpeedMultiplier / phase2SpeedMultiplier); // relative to phase 2
        }

        // Change sprite color to red (enraged)
        if (spriteRenderer != null)
        {
            spriteRenderer.color = phase3Color;
        }

        // Reduce attack cooldown (faster attacks)
        Enemy_Combat combat = GetComponent<Enemy_Combat>();
        if (combat != null)
        {
            // If Enemy_Combat has a cooldown field, multiply it here
            // Example: combat.attackCooldown *= phase3AttackCooldownMultiplier;
        }

        // Summon minions immediately
        if (minionPrefab != null)
        {
            SummonMinions();
            nextSummonTime = Time.time + (summonCooldown * 0.5f); // summon more frequently in phase 3
        }
    }

    /// <summary>
    /// Spawns minions at designated spawn points.
    /// </summary>
    public void SummonMinions()
    {
        if (minionPrefab == null)
        {
            Debug.LogWarning("Boss_Behavior: Cannot summon minions - missing prefab.");
            return;
        }

        Debug.Log($"Boss summoning {minionsPerSummon} minions!");

        for (int i = 0; i < minionsPerSummon; i++)
        {
            // Spawn minion at a random offset near the boss (behind or around)
            Vector3 offset = GetRandomMinionOffset();
            Vector3 spawnPos = transform.position + offset;
            GameObject minion = Instantiate(minionPrefab, spawnPos, Quaternion.identity);

            // Optional: make minions slightly weaker than regular enemies
            Enemy_Health minionHealth = minion.GetComponent<Enemy_Health>();
            if (minionHealth != null)
            {
                minionHealth.maxHealth = Mathf.Max(1, minionHealth.maxHealth / 2); // half health
                minionHealth.currentHealth = minionHealth.maxHealth;
            }
        }
    }

    // Helper: get a random offset behind or near the boss
    private Vector3 GetRandomMinionOffset()
    {
        // Spawn minion behind or to the sides of the boss (randomized)
        float distance = Random.Range(1.0f, 2.5f); // 1-2.5 units away
        float angle = Random.Range(120f, 240f) * Mathf.Deg2Rad; // behind the boss (120-240 degrees)
        float x = Mathf.Cos(angle) * distance;
        float y = Mathf.Sin(angle) * distance;
        return new Vector3(x, y, 0);
    }

    /// <summary>
    /// Call this from an animation event or manually to trigger a summon.
    /// </summary>
    public void TriggerManualSummon()
    {
        SummonMinions();
    }
}
