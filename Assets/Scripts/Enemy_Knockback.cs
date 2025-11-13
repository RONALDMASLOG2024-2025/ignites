using System.Collections;
using UnityEngine;

public class Enemy_Knockback : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created


    private Rigidbody2D rb;
    private Enemy_Movement enemyMovement;


    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        enemyMovement = GetComponent<Enemy_Movement>();
    }


    public void Knockback(Transform playerTransform, float knockback, float knockbackTime, float stunTime)
    {
        enemyMovement.ChangeState(EnemyState.Knockback);
        StartCoroutine( StunTimer(knockbackTime, stunTime));
        Vector2 direction = (transform.position - playerTransform.position).normalized;
        rb.linearVelocity = direction * knockback;
        Debug.Log("Knockback applied with force: " + knockback);

    }

    IEnumerator StunTimer(float knockbackTime, float stunTime)
    {
        yield return new WaitForSeconds(knockbackTime);
        rb.linearVelocity = Vector2.zero;
         yield return new WaitForSeconds(stunTime);
        enemyMovement.ChangeState(EnemyState.Idle);
    }

}
