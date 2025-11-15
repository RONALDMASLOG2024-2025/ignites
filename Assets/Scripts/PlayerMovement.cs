using System.Collections;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float speed = 5f;
    public int facingDirection = 1;
    public Rigidbody2D rb;
    public Animator animator;

    [Header("Footstep Sound")]
    public AudioSource footstepSource; // Drag AudioSource here
    public AudioClip footstepClip;     // Assign your footsteps-stairs-fast-90220.mp3



    private bool isKnockback;

    public Player_Combat playerCombat;



    private void Update()
    {
        // Always respect centralized GameState for pause when available
        if (GameState.Instance != null && GameState.Instance.IsPaused)
        {
            return;
        }

        if (Input.GetMouseButtonDown(0))
        {
            playerCombat.Attack1();
        }

        if (Input.GetMouseButtonDown(1))
        {
            playerCombat.Attack2();
        }
    }

    void FixedUpdate()
    {
        // Always respect centralized GameState for pause when available
        if (GameState.Instance != null && GameState.Instance.IsPaused)
        {
            return;
        }

        if (isKnockback == false)
        {
            float horizontal = Input.GetAxisRaw("Horizontal");
            float vertical = Input.GetAxisRaw("Vertical");

            if ((horizontal > 0 && transform.localScale.x < 0) || (horizontal < 0 && transform.localScale.x > 0))
            {
                Flip();
            }

            // Apply movement
            Vector2 move = new Vector2(horizontal, vertical).normalized * speed;
            rb.linearVelocity = move;

            // Animator values
            animator.SetFloat("horizontal", Mathf.Abs(horizontal));
            animator.SetFloat("vertical", Mathf.Abs(vertical));

            // Footstep Sound Logic
            if (move.magnitude > 0.1f)
            {
                if (!footstepSource.isPlaying)
                {
                    Debug.Log("Playing footstep sound");
                    footstepSource.clip = footstepClip;
                    footstepSource.loop = true;
                    footstepSource.Play();
                }
            }
            else
            {
                if (footstepSource.isPlaying)
                {
                    footstepSource.Stop();
                }
            }
        }
    }





    void Flip()
    {
        facingDirection *= -1;
        transform.localScale = new Vector3(transform.localScale.x * -1, transform.localScale.y, transform.localScale.z);
    }

    public void knockback(Transform enemy, float knockbackForce, float stunTime)
    {
        isKnockback = true;
        Vector2 direction = (transform.position - enemy.position).normalized;
        rb.linearVelocity = direction * knockbackForce; // Adjust knockback force as needed
        StartCoroutine(knockbackCounter(stunTime));
        // Implement knockback logic here
    }

    IEnumerator knockbackCounter(float stunTime)
    {
        yield return new WaitForSeconds(stunTime);
        rb.linearVelocity = Vector2.zero;
        isKnockback = false;
    }
}
