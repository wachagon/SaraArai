using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public Transform target;
    public GameObject projectilePrefab;
    public float attackInterval = 2f;
    public int projectilesPerAttack = 3;
    public float landingRandomRadius = 2f;
    public GameObject soldierPrefab;
    public int soldiersPerSummon = 4;
    public float soldierSpawnRadius = 1.5f;
    [Range(0f, 1f)] public float summonChance = 0.5f;
    public bool summonOnlyOnce = false;

    private float attackTimer;
    private bool hasSummonedSoldiers;

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
        if (target == null)
        {
            return;
        }

        attackTimer -= Time.deltaTime;

        if (attackTimer <= 0f)
        {
            if (ChooseRandomAction())
            {
                attackTimer = attackInterval;
            }
        }
    }

    bool ChooseRandomAction()
    {
        bool canShoot = projectilePrefab != null;
        bool canSummon = soldierPrefab != null && (!summonOnlyOnce || !hasSummonedSoldiers);

        if (!canShoot && !canSummon)
        {
            return false;
        }

        if (canShoot && canSummon)
        {
            if (Random.value < summonChance)
            {
                SummonSoldiers();
            }
            else
            {
                ShootArcProjectilesNearTarget();
            }

            return true;
        }

        if (canSummon)
        {
            SummonSoldiers();
        }
        else
        {
            ShootArcProjectilesNearTarget();
        }

        return true;
    }

    void ShootArcProjectilesNearTarget()
    {
        for (int i = 0; i < projectilesPerAttack; i++)
        {
            Vector2 randomOffset = Random.insideUnitCircle * landingRandomRadius;
            Vector2 landingPosition = (Vector2)target.position + randomOffset;
            landingPosition = new Vector2(Mathf.Round(landingPosition.x), Mathf.Round(landingPosition.y));

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

    void SummonSoldiers()
    {
        if (summonOnlyOnce && hasSummonedSoldiers)
        {
            return;
        }

        hasSummonedSoldiers = true;

        for (int i = 0; i < soldiersPerSummon; i++)
        {
            float angle = i * Mathf.PI * 2f / soldiersPerSummon;
            Vector2 offset = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)) * soldierSpawnRadius;
            GameObject soldier = Instantiate(soldierPrefab, (Vector2)transform.position + offset, Quaternion.identity);
            SummonedSoldier summonedSoldier = soldier.GetComponent<SummonedSoldier>();

            if (summonedSoldier != null)
            {
                summonedSoldier.SetTarget(target);
            }
        }
    }
}

