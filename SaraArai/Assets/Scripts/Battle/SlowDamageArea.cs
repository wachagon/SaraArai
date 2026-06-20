using UnityEngine;

public class SlowDamageArea : MonoBehaviour
{
    public float duration = 4f;
    public float fadeOutDuration = 1f;
    public float damagePerSecond = 5f;
    public float slowMultiplier = 0.5f;
    public float slowRefreshTime = 0.15f;

    private SpriteRenderer[] spriteRenderers;
    private Color[] startColors;
    private float timer;

    void Start()
    {
        spriteRenderers = GetComponentsInChildren<SpriteRenderer>();
        startColors = new Color[spriteRenderers.Length];

        for (int i = 0; i < spriteRenderers.Length; i++)
        {
            startColors[i] = spriteRenderers[i].color;
        }

        Destroy(gameObject, duration);
    }

    void Update()
    {
        timer += Time.deltaTime;

        if (fadeOutDuration <= 0f || spriteRenderers.Length == 0)
        {
            return;
        }

        float fadeStartTime = duration - fadeOutDuration;
        if (timer < fadeStartTime)
        {
            return;
        }

        float fadeProgress = Mathf.InverseLerp(fadeStartTime, duration, timer);
        float alphaMultiplier = 1f - fadeProgress;

        for (int i = 0; i < spriteRenderers.Length; i++)
        {
            Color color = startColors[i];
            color.a *= alphaMultiplier;
            spriteRenderers[i].color = color;
        }
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
