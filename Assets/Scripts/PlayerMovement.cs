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

    [Header("Attack Sounds")]
    public AudioSource attackSource;   // Another AudioSource for attack sounds
    public AudioClip attackClip1;      // Assign your attack sound (e.g., sword slash)
    public AudioClip attackClip2;      // Assign your second attack sound (e.g., heavy slash)

    void Update()
    {
        HandleAttackInput();
    }

    void FixedUpdate()
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

    private float attack1Cooldown = 0.7f; // Cooldown in seconds for attack 1
    private float attack2Cooldown = 0.7f; // Cooldown in seconds for attack 2
    private float lastAttack1Time = -Mathf.Infinity;
    private float lastAttack2Time = -Mathf.Infinity;

    void HandleAttackInput()
    {
        if (Input.GetMouseButtonDown(0) && Time.time - lastAttack1Time >= attack1Cooldown)
        {
            animator.SetTrigger("Attack");

            // Play attack sound 1
            if (attackClip1 != null)
                attackSource.PlayOneShot(attackClip1);

            lastAttack1Time = Time.time;
        }

        if (Input.GetMouseButtonDown(1) && Time.time - lastAttack2Time >= attack2Cooldown)
        {
            animator.SetTrigger("Attack-2");

            // Play attack sound 2
            if (attackClip2 != null)
                attackSource.PlayOneShot(attackClip2);

            lastAttack2Time = Time.time;
        }
    }

    void Flip()
    {
        facingDirection *= -1;
        transform.localScale = new Vector3(transform.localScale.x * -1, transform.localScale.y, transform.localScale.z);
    }
}
