using UnityEngine;
using UnityEngine.XR;

public class Enemy_Movement : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    public float speed;
    public float attackRange = 1f;


    public float attackCooldown = 1f;
    public int facingDirection = -1;
    private float attackCooldownTimer;

    public float playerDetectionRange = 5f;
    public Transform detectionPoint;


    public LayerMask playerLayer;


    private Rigidbody2D rb;
    private Transform player;




    private EnemyState currentState;



    private Animator animator;
    [Header("Audio")]
    public AudioClip footstepClip;
    [Range(0f, 1f)]
    public float footstepVolume = 0.5f;
    [Range(1f, 50f)]
    public float maxFootstepDistance = 20f; // How far footsteps can be heard
    private AudioSource footstepSource;
    
    void Start()
    {


        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        ChangeState(EnemyState.Idle);

        // Setup footstep audio source with spatial audio settings
        footstepSource = GetComponent<AudioSource>();
        if (footstepSource == null && footstepClip != null)
        {
            footstepSource = gameObject.AddComponent<AudioSource>();
            footstepSource.playOnAwake = false;
            footstepSource.loop = true;
            footstepSource.volume = footstepVolume;
            footstepSource.spatialBlend = 1f; // Full 3D spatial audio
            footstepSource.minDistance = 1f;
            footstepSource.maxDistance = maxFootstepDistance;
            footstepSource.rolloffMode = AudioRolloffMode.Linear;
        }
        else if (footstepSource != null)
        {
            // Apply volume/spatial settings if AudioSource already exists
            footstepSource.volume = footstepVolume;
            footstepSource.spatialBlend = 1f;
            footstepSource.maxDistance = maxFootstepDistance;
        }

        // Subscribe to GameState events to stop audio on pause/game end
        if (GameState.Instance != null)
        {
            GameState.Instance.OnPauseChanged += HandlePauseChanged;
            GameState.Instance.OnGameOver += StopAllAudio;
            GameState.Instance.OnVictory += StopAllAudio;
        }

        Debug.Log("Detect the object: " + detectionPoint.position);
    }

    void ChangeStateSSSSS(EnemyState newState)
    {

        Debug.Log(" State for enemy: " + newState);
    }

    public void ChangeState(EnemyState newState)
    {

        Debug.Log("New State for enemy: " + newState);
        // turn off current state's animation
        if (currentState == EnemyState.Idle)
        {
            animator.SetBool("isIdle", false);
        }
        else if (currentState == EnemyState.Chasing)
        {

            animator.SetBool("isChasing", false);
        }
        else if (currentState == EnemyState.Attacking)
        {
            Debug.Log("Stopping attack animation " + newState);
            animator.SetBool("isAttacking", false);
        }
        // else if (currentState == EnemyState.Dead)
        // {

        //     animator.SetBool("isDead", false);
        // }


        currentState = newState;

        // turn on new state's animation
        if (currentState == EnemyState.Idle)
        {
            animator.SetBool("isIdle", true);
            // Stop footsteps when idle
            if (footstepSource != null && footstepSource.isPlaying)
                footstepSource.Stop();
        }
        else if (currentState == EnemyState.Chasing)
        {

            animator.SetBool("isChasing", true);
            // Footsteps will be handled in MoveEnemy
        }
        else if (currentState == EnemyState.Attacking)
        {

            animator.SetBool("isAttacking", true);
            // Stop footsteps during attack
            if (footstepSource != null && footstepSource.isPlaying)
                footstepSource.Stop();
        }
        // else if (currentState == EnemyState.Dead)
        // {

        //     animator.SetBool("isDead", true);
        // }

    }


    // Update is called once per frame
    void Update()
    {
        // Always respect centralized GameState for pause when available
        if (GameState.Instance != null && GameState.Instance.IsPaused)
        {
            // If paused, ensure footstep audio is stopped
            if (footstepSource != null && footstepSource.isPlaying)
                footstepSource.Stop();
            return;
        }

        if (currentState != EnemyState.Knockback)
        {
            CheckForPlayer();

            if (attackCooldownTimer > 0)
            {
                attackCooldownTimer -= Time.deltaTime;
            }

            if (currentState == EnemyState.Chasing)
            {
                Debug.Log("Chasinggggggg the player");
                MoveEnemy();
            }
            else if (currentState == EnemyState.Attacking)
            {
                rb.linearVelocity = Vector2.zero; // Stop moving when attacking
                // Ensure footsteps stop when attacking
                if (footstepSource != null && footstepSource.isPlaying)
                    footstepSource.Stop();
                Debug.Log("attackinggggg the player");
            }
            else
            {
                // Stop footsteps in other states (Idle, Knockback)
                if (footstepSource != null && footstepSource.isPlaying)
                    footstepSource.Stop();
            }
        }

    }

    void MoveEnemy()
    {


        if ((player.position.x > transform.position.x && transform.localScale.x < 0) || (player.position.x < transform.position.x && transform.localScale.x > 0))
        {
            facingDirection *= -1;
            Vector3 newScale = transform.localScale;
            newScale.x *= -1;
            transform.localScale = newScale;
        }
        rb.linearVelocity = new Vector2(player.position.x - transform.position.x, player.position.y - transform.position.y).normalized * speed;

        // Play footstep loop when moving
        if (footstepSource != null && footstepClip != null)
        {
            if (rb.linearVelocity.magnitude > 0.1f)
            {
                if (!footstepSource.isPlaying)
                {
                    footstepSource.clip = footstepClip;
                    footstepSource.loop = true;
                    footstepSource.Play();
                }
            }
            else
            {
                if (footstepSource.isPlaying)
                    footstepSource.Stop();
            }
        }
    }

    private void CheckForPlayer()
    {
        Collider2D[] hitPlayers = Physics2D.OverlapCircleAll(detectionPoint.position, playerDetectionRange, playerLayer);

        if (hitPlayers.Length > 0)
        {
            player = hitPlayers[0].transform;

            if (Vector2.Distance(transform.position, player.position) <= attackRange && attackCooldownTimer <= 0f)
            {
                attackCooldownTimer = attackCooldown;
                ChangeState(EnemyState.Attacking);

            }
            else if (Vector2.Distance(transform.position, player.position) > attackRange && currentState != EnemyState.Attacking)
            {
                ChangeState(EnemyState.Chasing);
            }


        }
        else
        {
            rb.linearVelocity = Vector2.zero; // Stop moving when player exits the trigger
            // Stop footsteps when idle
            if (footstepSource != null && footstepSource.isPlaying)
                footstepSource.Stop();
            ChangeState(EnemyState.Idle);
        }
    }

    private void OnDrawGizmosSelected()
    {


        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(detectionPoint.position, playerDetectionRange);

        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(detectionPoint.position, attackRange);
    }

    private void OnDestroy()
    {
        // Unsubscribe from GameState events to prevent memory leaks
        if (GameState.Instance != null)
        {
            GameState.Instance.OnPauseChanged -= HandlePauseChanged;
            GameState.Instance.OnGameOver -= StopAllAudio;
            GameState.Instance.OnVictory -= StopAllAudio;
        }
    }

    private void HandlePauseChanged(bool isPaused)
    {
        if (isPaused)
        {
            StopAllAudio();
        }
    }

    private void StopAllAudio()
    {
        if (footstepSource != null && footstepSource.isPlaying)
        {
            footstepSource.Stop();
        }
    }

}



public enum EnemyState
{
    Idle,
    Chasing,
    Attacking,
    Knockback
    // Dead
}