using UnityEngine;

public class SummonedSoldier : MonoBehaviour
{
    public float maxHp = 30f;
    public Transform target;
    public float moveSpeed = 3f;
    public float contactDamage = 10f;
    public float damageInterval = 0.8f;

    private Rigidbody2D rb;
    private float currentHp;
    private float damageTimer;
    private bool isDefeated;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        currentHp = maxHp;

        if (target == null)
        {
            Player player = FindObjectOfType<Player>();
            if (player != null)
            {
                target = player.transform;
            }
        }
    }

    void Update()
    {
        if (isDefeated)
        {
            return;
        }

        if (damageTimer > 0f)
        {
            damageTimer -= Time.deltaTime;
        }
    }

    void FixedUpdate()
    {
        if (isDefeated)
        {
            return;
        }

        if (rb == null || target == null)
        {
            return;
        }

        Vector2 direction = ((Vector2)target.position - rb.position).normalized;
        rb.velocity = direction * moveSpeed;
    }

    void OnCollisionStay2D(Collision2D collision)
    {
        TryDamagePlayer(collision.gameObject);
    }

    void OnTriggerStay2D(Collider2D other)
    {
        TryDamagePlayer(other.gameObject);
    }

    public void SetTarget(Transform newTarget)
    {
        target = newTarget;
    }

    public void TakeDamage(float damage)
    {
        if (isDefeated)
        {
            return;
        }

        currentHp = Mathf.Max(0f, currentHp - damage);

        if (currentHp <= 0f)
        {
            Defeat();
        }
    }

    void Defeat()
    {
        isDefeated = true;
        Destroy(gameObject);
    }

    void TryDamagePlayer(GameObject other)
    {
        if (damageTimer > 0f)
        {
            return;
        }

        Player player = other.GetComponent<Player>();
        if (player == null)
        {
            return;
        }

        player.TakeDamage(contactDamage);
        damageTimer = damageInterval;
    }
}
