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
    void Start()
    {


        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        ChangeState(EnemyState.Idle);

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
        }
        else if (currentState == EnemyState.Chasing)
        {

            animator.SetBool("isChasing", true);
        }
        else if (currentState == EnemyState.Attacking)
        {

            animator.SetBool("isAttacking", true);
        }
        // else if (currentState == EnemyState.Dead)
        // {

        //     animator.SetBool("isDead", true);
        // }

    }


    // Update is called once per frame
    void Update()
    {
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
                Debug.Log("attackinggggg the player");
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

}



public enum EnemyState
{
    Idle,
    Chasing,
    Attacking,

    Knockback
    // Dead
}