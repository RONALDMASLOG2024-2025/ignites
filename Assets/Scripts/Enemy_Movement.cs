using UnityEngine;
using UnityEngine.XR;

public class Enemy_Movement : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    public float speed;
    private Rigidbody2D rb;
    private Transform player;


    private EnemyState currentState;

    public int facingDirection = -1;

    private Animator animator;
    void Start()
    {


        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        ChangeState(EnemyState.Idle);
    }

    void ChangeState(EnemyState newState)
    {
        if (currentState == EnemyState.Idle)
        {
            animator.SetBool("isIdle", false);
        }
        else if (currentState == EnemyState.Chasing)
        {

            animator.SetBool("isChasing", false);
        }


        currentState = newState;

        if (currentState == EnemyState.Idle)
        {
            animator.SetBool("isIdle", true);
        }
        else if (currentState == EnemyState.Chasing)
        {

            animator.SetBool("isChasing", true);
        }

    }


    // Update is called once per frame
    void Update()
    {
        if (currentState == EnemyState.Chasing)
        {
            if ((player.position.x > transform.position.x && transform.localScale.x < 0) || (player.position.x < transform.position.x && transform.localScale.x > 0))
            {
                facingDirection *= -1;
                Vector3 newScale = transform.localScale;
                newScale.x *= -1;
                transform.localScale = newScale;
            }
            MoveEnemy();
        }
    }

    void MoveEnemy()
    {
        rb.linearVelocity = new Vector2(player.position.x - transform.position.x, player.position.y - transform.position.y).normalized * speed;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {


            if (player == null)
            {
                player = collision.transform;
            }
            ChangeState(EnemyState.Chasing);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {


            rb.linearVelocity = Vector2.zero; // Stop moving when player exits the trigger
            ChangeState(EnemyState.Idle);
        }
    }

}



public enum EnemyState
{
    Idle,
    Chasing
}