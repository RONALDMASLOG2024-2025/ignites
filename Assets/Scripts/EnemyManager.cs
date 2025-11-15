using UnityEngine;
using TMPro;
using System.Collections;
using Unity.Cinemachine;

/// <summary>
/// Singleton manager that tracks all regular enemies in the scene.
/// When all enemies are killed, spawns the boss with entrance sequence.
/// </summary>
public class EnemyManager : MonoBehaviour
{
    public static EnemyManager Instance { get; private set; }

    [Header("Enemy Tracking")]
    [Tooltip("Leave empty - enemies register themselves automatically")]
    private int remainingEnemies = 0;

    [Header("UI")]
    public TMP_Text enemyCountText;
    public BossHealthBar bossHealthBar;

    [Header("Boss Spawn")]
    public GameObject bossPrefab;
    public Transform bossSpawnPoint;
    
    [Header("Camera - Cinemachine 3.x")]
    [Tooltip("The main Cinemachine camera that follows the player")]
    public CinemachineCamera playerCamera;
    
    [Tooltip("Optional: A secondary camera positioned at boss spawn for entrance shot")]
    public CinemachineCamera bossEntranceCamera;
    
    [Tooltip("How long to show the boss entrance before returning to player (seconds)")]
    public float bossEntranceDuration = 3f;

    [Header("Boss Entrance Effects")]
    public bool playBossMusic = true;
    public AudioClip bossSpawnSound; // optional sound effect when boss appears
    private AudioSource audioSource;
    
    private bool bossSpawned = false;
    private GameObject spawnedBoss;

    private void Awake()
    {
        // Singleton pattern
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Debug.LogWarning("Multiple EnemyManagers detected! Destroying duplicate.");
            Destroy(gameObject);
            return;
        }

        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
    }

    private void Start()
    {
        UpdateUI();
        
        // Ensure boss entrance camera is disabled initially
        if (bossEntranceCamera != null)
        {
            bossEntranceCamera.gameObject.SetActive(false);
        }
    }

    /// <summary>
    /// Called by enemies when they spawn/awake. Increments the counter.
    /// </summary>
    public void RegisterEnemy()
    {
        remainingEnemies++;
        UpdateUI();
        Debug.Log($"Enemy registered. Total enemies: {remainingEnemies}");
    }

    /// <summary>
    /// Called by enemies when they die. Decrements the counter.
    /// If count reaches zero, spawns the boss.
    /// </summary>
    public void UnregisterEnemy()
    {
        remainingEnemies--;
        UpdateUI();
        Debug.Log($"Enemy killed. Remaining enemies: {remainingEnemies}");

        if (remainingEnemies <= 0 && !bossSpawned)
        {
            StartCoroutine(BossEntranceSequence());
        }
    }

    private void UpdateUI()
    {
        if (enemyCountText != null)
        {
            enemyCountText.text = $"Enemies: {remainingEnemies}";
        }
    }

    /// <summary>
    /// Boss entrance sequence with camera work and effects.
    /// </summary>
    private IEnumerator BossEntranceSequence()
    {
        bossSpawned = true;
        Debug.Log("🔥 All enemies defeated! Boss entrance starting...");

        // 1. Switch to boss entrance camera if available
        if (bossEntranceCamera != null && playerCamera != null)
        {
            // Disable player camera, enable boss entrance camera
            playerCamera.gameObject.SetActive(false);
            bossEntranceCamera.gameObject.SetActive(true);
            
            Debug.Log("Camera focusing on boss spawn point...");
        }

        // 2. Wait a moment before spawning (dramatic pause)
        yield return new WaitForSeconds(0.5f);

        // 3. Spawn the boss
        if (bossPrefab != null && bossSpawnPoint != null)
        {
            spawnedBoss = Instantiate(bossPrefab, bossSpawnPoint.position, Quaternion.identity);
            Debug.Log($"⚔️ BOSS SPAWNED at {bossSpawnPoint.position}");

            // Play boss spawn sound
            if (bossSpawnSound != null && audioSource != null)
            {
                audioSource.PlayOneShot(bossSpawnSound);
            }

            // Link boss to health bar
            if (bossHealthBar != null)
            {
                Enemy_Health bossHealth = spawnedBoss.GetComponent<Enemy_Health>();
                if (bossHealth != null)
                {
                    bossHealthBar.SetBoss(bossHealth);
                }
            }
        }
        else
        {
            Debug.LogWarning("Boss spawn skipped: bossPrefab or bossSpawnPoint not assigned.");
        }

        // 4. Switch to boss music
        if (playBossMusic && MusicManager.Instance != null)
        {
            MusicManager.Instance.PlayBossMusic();
        }

        // 5. Wait for entrance duration (let player see the boss)
        yield return new WaitForSeconds(bossEntranceDuration);

        // 6. Return camera to player
        if (bossEntranceCamera != null && playerCamera != null)
        {
            bossEntranceCamera.gameObject.SetActive(false);
            playerCamera.gameObject.SetActive(true);
            Debug.Log("Camera returned to player.");
        }

        Debug.Log("Boss entrance complete. Fight begins!");
    }

    /// <summary>
    /// Optional: Call this to manually reset the enemy count (for testing or scene reloads).
    /// </summary>
    public void ResetEnemyCount()
    {
        remainingEnemies = 0;
        bossSpawned = false;
        UpdateUI();
        
        if (bossHealthBar != null)
        {
            bossHealthBar.HideHealthBar();
        }
    }

    /// <summary>
    /// Call this when boss is defeated to return to normal music.
    /// </summary>
    public void OnBossDefeated()
    {
        Debug.Log("Boss has been defeated!");
        
        if (MusicManager.Instance != null)
        {
            MusicManager.Instance.PlayNormalMusic();
        }

        if (bossHealthBar != null)
        {
            bossHealthBar.HideHealthBar();
        }
    }
}
