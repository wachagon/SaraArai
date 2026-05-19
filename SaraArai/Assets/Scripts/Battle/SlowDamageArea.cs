using UnityEngine;

public class SlowDamageArea : MonoBehaviour
{
    public float duration = 4f;
    public float damagePerSecond = 5f;
    public float slowMultiplier = 0.5f;
    public float slowRefreshTime = 0.15f;

    void Start()
    {
        Destroy(gameObject, duration);
    }

    void OnTriggerStay2D(Collider2D other)
    {
        Player player = other.GetComponent<Player>();
        if (player == null)
        {
            return;
        }

        player.TakeDamage(damagePerSecond * Time.deltaTime);
        player.ApplySlow(slowMultiplier, slowRefreshTime);
    }
}
