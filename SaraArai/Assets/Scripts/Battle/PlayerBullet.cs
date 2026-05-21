using UnityEngine;

public class PlayerBullet : MonoBehaviour
{
    private float damage = 10f;

    public void SetDamage(float newDamage)
    {
        damage = newDamage;
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        TryDamage(collision.gameObject);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        TryDamage(other.gameObject);
    }

    void TryDamage(GameObject other)
    {
        Enemy enemy = other.GetComponent<Enemy>();
        if (enemy != null)
        {
            enemy.TakeDamage(damage);
            Destroy(gameObject);
            return;
        }

        SummonedSoldier soldier = other.GetComponent<SummonedSoldier>();
        if (soldier != null)
        {
            soldier.TakeDamage(damage);
            Destroy(gameObject);
        }
    }
}
