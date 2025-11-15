using UnityEngine;

/// <summary>
/// Meat pickup that heals the player when collected.
/// Attach to a meat prefab with a Collider2D (trigger) and SpriteRenderer.
/// </summary>
public class MeatPickup : MonoBehaviour
{
    [Header("Heal Settings")]
    [Tooltip("How much health the meat restores")]
    public int healAmount = 1;

    [Header("Pickup Settings")]
    public bool destroyOnPickup = true;
    public AudioClip pickupSound; // optional sound effect
    [Range(0f, 1f)]
    public float pickupVolume = 0.6f;
    [Range(1f, 50f)]
    public float maxPickupDistance = 20f;

    private AudioSource audioSource;

    private void Start()
    {
        // Get or add AudioSource for pickup sound
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null && pickupSound != null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
            audioSource.playOnAwake = false;
        }

        // Ensure this has a trigger collider
        Collider2D col = GetComponent<Collider2D>();
        if (col != null)
        {
            col.isTrigger = true;
        }
        else
        {
            Debug.LogWarning("MeatPickup: No Collider2D found! Add a CircleCollider2D or BoxCollider2D and set it as trigger.");
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Check if player collided with the meat
        if (collision.CompareTag("Player"))
        {
            PlayerHealth playerHealth = collision.GetComponent<PlayerHealth>();
            if (playerHealth != null)
            {
                // Heal the player
                playerHealth.UpdateHealth(healAmount);
                Debug.Log($"Player picked up meat! Healed for {healAmount} HP.");

                // Play pickup sound. If the meat GameObject will be destroyed immediately
                // after pickup, PlayOneShot on its AudioSource may be cut off. Use
                // PlayClipAtPoint so the sound continues even after this object is
                // destroyed. If destroyOnPickup is false we can safely PlayOneShot.
                // Don't play if game is paused or ended
                if (pickupSound != null && (GameState.Instance == null || (!GameState.Instance.IsPaused && !GameState.Instance.IsGameEnded)))
                {
                    if (!destroyOnPickup && audioSource != null)
                    {
                        audioSource.PlayOneShot(pickupSound);
                    }
                    else
                    {
                        // Create a temporary spatial AudioSource for the pickup sound
                        GameObject audioObj = new GameObject("PickupSound");
                        audioObj.transform.position = transform.position;
                        AudioSource tempSource = audioObj.AddComponent<AudioSource>();
                        tempSource.clip = pickupSound;
                        tempSource.volume = pickupVolume;
                        tempSource.spatialBlend = 1f; // Full 3D spatial audio
                        tempSource.minDistance = 1f;
                        tempSource.maxDistance = maxPickupDistance;
                        tempSource.rolloffMode = AudioRolloffMode.Linear;
                        tempSource.Play();
                        Destroy(audioObj, pickupSound.length + 0.1f);
                    }
                }

                // Destroy or disable the meat
                if (destroyOnPickup)
                {
                    Destroy(gameObject);
                }
                else
                {
                    gameObject.SetActive(false);
                }
            }
        }
    }
}
