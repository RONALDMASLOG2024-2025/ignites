using UnityEngine;

/// <summary>
/// Bone pickup that permanently enhances player stats (damage and/or movement speed).
/// Attach to a bone prefab with a Collider2D (trigger) and SpriteRenderer.
/// 
/// Usage:
/// - Set damageBoost to increase player's attack damage
/// - Set speedBoost to increase player's movement speed
/// - Bonuses are permanent for the current level (reset on level reload)
/// </summary>
public class BonePickup : MonoBehaviour
{
    [Header("Stat Enhancement")]
    [Tooltip("How much to increase player's attack damage (added to Player_Combat.Damage)")]
    public int damageBoost = 1;
    
    [Tooltip("How much to increase player's movement speed (added to PlayerMovement.speed)")]
    [Range(0f, 5f)]
    public float speedBoost = 0.5f;

    [Header("Pickup Settings")]
    public bool destroyOnPickup = true;
    public AudioClip pickupSound; // optional sound effect
    [Range(0f, 1f)]
    public float pickupVolume = 0.7f;
    [Range(1f, 50f)]
    public float maxPickupDistance = 20f;

    [Header("Visual Feedback (Optional)")]
    public GameObject pickupEffect; // Optional particle effect on pickup

    private void Start()
    {
        // Ensure this has a trigger collider
        Collider2D col = GetComponent<Collider2D>();
        if (col != null)
        {
            col.isTrigger = true;
        }
        else
        {
            Debug.LogWarning("BonePickup: No Collider2D found! Add a CircleCollider2D or BoxCollider2D and set it as trigger.");
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Check if player collided with the bone
        if (collision.CompareTag("Player"))
        {
            // Get player components
            PlayerMovement playerMovement = collision.GetComponent<PlayerMovement>();
            Player_Combat playerCombat = collision.GetComponent<Player_Combat>();

            bool statsChanged = false;
            string pickupMessage = "Player picked up bone!";

            // Apply damage boost
            if (damageBoost > 0 && playerCombat != null)
            {
                playerCombat.Damage += damageBoost;
                pickupMessage += $" Damage +{damageBoost} (Now: {playerCombat.Damage})";
                statsChanged = true;
            }

            // Apply speed boost
            if (speedBoost > 0 && playerMovement != null)
            {
                playerMovement.speed += speedBoost;
                pickupMessage += $" Speed +{speedBoost:F1} (Now: {playerMovement.speed:F1})";
                statsChanged = true;
            }

            if (statsChanged)
            {
                Debug.Log(pickupMessage);

                // Play pickup sound
                PlayPickupSound();

                // Spawn visual effect if assigned
                if (pickupEffect != null)
                {
                    Instantiate(pickupEffect, transform.position, Quaternion.identity);
                }

                // Destroy or disable the bone
                if (destroyOnPickup)
                {
                    Destroy(gameObject);
                }
                else
                {
                    gameObject.SetActive(false);
                }
            }
            else
            {
                Debug.LogWarning("BonePickup: No stat boosts configured! Set damageBoost and/or speedBoost.");
            }
        }
    }

    private void PlayPickupSound()
    {
        // Don't play if game is paused or ended
        if (GameState.Instance != null && (GameState.Instance.IsPaused || GameState.Instance.IsGameEnded))
        {
            return;
        }

        if (pickupSound == null) return;

        // Create a temporary spatial AudioSource for the pickup sound
        // This ensures sound continues even after the bone is destroyed
        GameObject audioObj = new GameObject("BonePickupSound");
        audioObj.transform.position = transform.position;
        
        AudioSource tempSource = audioObj.AddComponent<AudioSource>();
        tempSource.clip = pickupSound;
        tempSource.volume = pickupVolume;
        tempSource.spatialBlend = 1f; // Full 3D spatial audio
        tempSource.minDistance = 1f;
        tempSource.maxDistance = maxPickupDistance;
        tempSource.rolloffMode = AudioRolloffMode.Linear;
        tempSource.Play();
        
        // Destroy audio object after sound finishes
        Destroy(audioObj, pickupSound.length + 0.1f);
    }
}
