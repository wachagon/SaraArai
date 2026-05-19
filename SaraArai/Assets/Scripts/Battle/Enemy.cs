using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public Transform target;
    public GameObject projectilePrefab;
    public float attackInterval = 2f;
    public float attackRange = 8f;
    public int projectilesPerAttack = 3;
    public float landingRandomRadius = 2f;

    private float attackTimer;

    void Start()
    {
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
        if (target == null || projectilePrefab == null)
        {
            return;
        }

        attackTimer -= Time.deltaTime;

        float distance = Vector2.Distance(transform.position, target.position);
        if (attackTimer <= 0f && distance <= attackRange)
        {
            ShootArcProjectilesNearTarget();
            attackTimer = attackInterval;
        }
    }

    void ShootArcProjectilesNearTarget()
    {
        for (int i = 0; i < projectilesPerAttack; i++)
        {
            Vector2 randomOffset = Random.insideUnitCircle * landingRandomRadius;
            Vector2 landingPosition = (Vector2)target.position + randomOffset;

            ShootArcProjectile(landingPosition);
        }
    }

    void ShootArcProjectile(Vector2 landingPosition)
    {
        GameObject projectile = Instantiate(projectilePrefab, transform.position, Quaternion.identity);
        EnemyProjectile enemyProjectile = projectile.GetComponent<EnemyProjectile>();

        if (enemyProjectile != null)
        {
            enemyProjectile.Launch(transform.position, landingPosition);
        }
    }
}
