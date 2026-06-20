using UnityEngine;

public class EnemyProjectile : MonoBehaviour
{
    public GameObject slowDamageAreaPrefab;
    public float flightTime = 0.8f;
    public float arcHeight = 2f;
    public float maxLifetime = 3f;

    private Vector2 startPosition;
    private Vector2 landingPosition;
    private float timer;
    private bool launched;

    public void Launch(Vector2 start, Vector2 landing)
    {
        startPosition = start;
        landingPosition = landing;
        timer = 0f;
        launched = true;

        Destroy(gameObject, maxLifetime);
    }

    void Update()
    {
        if (!launched)
        {
            return;
        }

        timer += Time.deltaTime;
        float t = Mathf.Clamp01(timer / flightTime);

        Vector2 groundPosition = Vector2.Lerp(startPosition, landingPosition, t);
        float height = 4f * arcHeight * t * (1f - t);
        transform.position = groundPosition + Vector2.up * height;

        if (t >= 1f)
        {
            Land();
        }
    }

    void Land()
    {
        if (slowDamageAreaPrefab != null)
        {
            Instantiate(slowDamageAreaPrefab, landingPosition, Quaternion.identity);
        }

        Destroy(gameObject);
    }
}
