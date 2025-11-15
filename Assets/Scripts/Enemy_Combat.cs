using Unity.InferenceEngine;
using UnityEngine;

public class Enemy_Combat : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    public int damage = 1;
    public Transform attackPoint;
    public float weaponRange;
    public float knockbackForce = 5f;
    public float stunTime = 0.2f;

    public LayerMask playerLayer;
    [Header("Audio")]
    public AudioClip attackSound;
    [Range(0f, 1f)]
    public float attackVolume = 0.7f;
    [Range(1f, 50f)]
    public float maxAttackDistance = 25f; // How far attack sounds can be heard




    // private void OnCollisionEnter2D(Collision2D collision)
    // {
    //     if (collision.gameObject.tag == "Player")
    //     {

    //         PlayerHealth playerHealth = collision.gameObject.GetComponent<PlayerHealth>();
    //         if (playerHealth != null)
    //         {
    //             playerHealth.UpdateHealth(-damage); // Decrease player health by damage amount
    //             Debug.Log("Player hit by enemy! Health decreased.");
    //         }
    //     }

    // }

    public void Attack()
    {
        // Don't attack while the game is paused or has ended
        // Always use GameState when available for consistent behavior
        if (GameState.Instance != null && (GameState.Instance.IsPaused || GameState.Instance.IsGameEnded))
        {
            return;
        }

        // This method can be called from an animation event
        Debug.Log("Enemy attack animation event triggered.");

        // Play attack SFX with spatial audio and adjustable volume
        // Don't play if game is paused or ended
        if (attackSound != null && GameState.Instance != null && !GameState.Instance.IsPaused && !GameState.Instance.IsGameEnded)
        {
            GameObject audioObj = new GameObject("AttackSound");
            audioObj.transform.position = transform.position;
            AudioSource audioSource = audioObj.AddComponent<AudioSource>();
            audioSource.clip = attackSound;
            audioSource.volume = attackVolume;
            audioSource.spatialBlend = 1f; // Full 3D spatial audio
            audioSource.minDistance = 1f;
            audioSource.maxDistance = maxAttackDistance;
            audioSource.rolloffMode = AudioRolloffMode.Linear;
            audioSource.Play();
            Destroy(audioObj, attackSound.length + 0.1f); // Clean up after sound finishes
        }
        
        Collider2D[] hitPlayers = Physics2D.OverlapCircleAll(attackPoint.position, weaponRange, playerLayer);
        if(hitPlayers.Length > 0)
        {
            var ph = hitPlayers[0].GetComponent<PlayerHealth>();
            if (ph != null)
            {
                ph.UpdateHealth(-damage);
                // Only attempt knockback if the player is still alive and active
                if (!ph.isDead && hitPlayers[0].gameObject.activeInHierarchy)
                {
                    var pm = hitPlayers[0].GetComponent<PlayerMovement>();
                    if (pm != null)
                    {
                        pm.knockback(transform, knockbackForce, stunTime);
                    }
                }
            }
            Debug.Log("Player hit by enemy attack! Health decreased.");
        }   
    }
}



