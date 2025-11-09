using UnityEngine;

public class Enemy_Combat : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    public int damage = 1;
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {

            PlayerHealth playerHealth = collision.gameObject.GetComponent<PlayerHealth>();
            if (playerHealth != null)
            {
                playerHealth.UpdateHealth(-damage); // Decrease player health by damage amount
                Debug.Log("Player hit by enemy! Health decreased.");
            }
        }

    }

    public void Attack()
    {
        // This method can be called from an animation event
        Debug.Log("Enemy attack animation event triggered.");
    }
}



