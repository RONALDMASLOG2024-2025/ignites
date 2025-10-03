using UnityEngine;

public class Enemy : MonoBehaviour
{
    public int maxHealth = 50;
    int currentHealth;
    public Animator animator;

    void Start()
    {
        currentHealth = maxHealth;
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;

        animator.SetTrigger("Hurt");

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        Debug.Log(name + " died!");
        animator.SetTrigger("Die");
        // Disable enemy
        GetComponent<Collider2D>().enabled = false;
        this.enabled = false;
    }
}
