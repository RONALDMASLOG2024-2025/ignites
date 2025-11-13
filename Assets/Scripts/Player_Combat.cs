using UnityEngine;

public class Player_Combat : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public Animator anim;
    public Transform attackPoint;
    public float weaponRange = 1f;
    public LayerMask enemyLayers;
    public int Damage = 1;


    public float attack1Cooldown = 1f; // Cooldown in seconds for attack 1
    // public float attack2Cooldown = 1f; // Cooldown in seconds for attack 2
    // // Remaining cooldown time. 0 = ready.
    private float timer1 = 0f;
    // private float timer2 = 0f;
    [Header("Attack Sounds")]
    public AudioSource attackSource;   // Another AudioSource for attack sounds
    public AudioClip attackClip1;      // Assign your attack sound (e.g., sword slash)
    // public AudioClip attackClip2;      // Assign your second attack sound (e.g., heavy slash)



    public float knockbackForce = 20f;
    public float knockbackTime = 0.2f;
    public float stunTime = 0.3f;

    private void Update()
    {
        if (timer1 > 0f)
        {
            timer1 -= Time.deltaTime;
            if (timer1 < 0f) timer1 = 0f; // clamp to zero
        }

        // if (timer2 > 0f)
        // {
        //     timer2 -= Time.deltaTime;
        //     if (timer2 < 0f) timer2 = 0f; // clamp to zero
        // }
    }
    public void Attack1()
    {
        // Only allow attack when cooldown has expired
        if (timer1 <= 0f)
        {
            if (anim != null)
                anim.SetBool("isAttacking1", true);


            // Play attack sound 1
            if (attackSource != null && attackClip1 != null)
                attackSource.PlayOneShot(attackClip1);

            timer1 = attack1Cooldown;
        }
    }


    public void DealDamage()
    {
        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(attackPoint.position, weaponRange, enemyLayers);

        if (hitEnemies.Length > 0)
        {
            hitEnemies[0].GetComponent<Enemy_Health>().ChangeHealth(-Damage);
            hitEnemies[0].GetComponent<Enemy_Knockback>().Knockback(transform, knockbackForce, knockbackTime, stunTime);
        }
    }

    // public void Attack2()
    // {
    //     // Only allow attack when cooldown has expired
    //     if (timer2 <= 0f)
    //     {
    //         if (anim != null)
    //             anim.SetBool("isAttacking2", true);

    //         // Play attack sound 2
    //         if (attackSource != null && attackClip2 != null)
    //             attackSource.PlayOneShot(attackClip2);

    //         timer2 = attack2Cooldown;
    //     }
    // }


    public void FinishAttack()
    {
        anim.SetBool("isAttacking1", false);
        // anim.SetBool("isAttacking2", false);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;

        Gizmos.DrawWireSphere(attackPoint.position, weaponRange);
    }


}
