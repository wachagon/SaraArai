using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public float maxHp = 100f;
    public Transform target;
    public Transform targetPos;
    public float moveSpeed = 2f;
    public float moveRange = 4f;
    public float arriveDistance = 0.1f;
    public float waitTimeAtTarget = 0.5f;
    public bool stayInsideCamera = true;
    public float cameraPadding = 0.5f;
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
    private float currentHp;
    private Vector2 startPosition;
    private float moveWaitTimer;
    private bool hasSummonedSoldiers;
    private bool createdTargetPos;
    private bool isDefeated;

    void Start()
    {
        currentHp = maxHp;
        startPosition = transform.position;
        SetupTargetPos();
        SetRandomTargetPos();

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

        MoveToTargetPos();

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

    void SetupTargetPos()
    {
        if (targetPos != null)
        {
            return;
        }

        GameObject targetPosObject = new GameObject(name + " TargetPos");
        targetPos = targetPosObject.transform;
        createdTargetPos = true;
    }

    void SetRandomTargetPos()
    {
        if (targetPos == null)
        {
            return;
        }

        Vector2 randomOffset = Random.insideUnitCircle * moveRange;
        Vector2 nextPosition = startPosition + randomOffset;
        targetPos.position = ClampToCamera(nextPosition);
    }

    void MoveToTargetPos()
    {
        if (targetPos == null)
        {
            return;
        }

        if (moveWaitTimer > 0f)
        {
            moveWaitTimer -= Time.deltaTime;
            return;
        }

        Vector2 currentPosition = transform.position;
        Vector2 targetPosition = targetPos.position;

        transform.position = Vector2.MoveTowards(
            currentPosition,
            targetPosition,
            moveSpeed * Time.deltaTime
        );

        if (stayInsideCamera)
        {
            transform.position = ClampToCamera(transform.position);
        }

        if (Vector2.Distance(transform.position, targetPosition) <= arriveDistance)
        {
            moveWaitTimer = waitTimeAtTarget;
            SetRandomTargetPos();
        }
    }

    Vector2 ClampToCamera(Vector2 position)
    {
        if (!stayInsideCamera || Camera.main == null)
        {
            return position;
        }

        Camera mainCamera = Camera.main;
        float distanceFromCamera = Mathf.Abs(mainCamera.transform.position.z - transform.position.z);
        Vector3 bottomLeft = mainCamera.ViewportToWorldPoint(new Vector3(0f, 0f, distanceFromCamera));
        Vector3 topRight = mainCamera.ViewportToWorldPoint(new Vector3(1f, 1f, distanceFromCamera));

        float minX = bottomLeft.x + cameraPadding;
        float maxX = topRight.x - cameraPadding;
        float minY = bottomLeft.y + cameraPadding;
        float maxY = topRight.y - cameraPadding;

        position.x = Mathf.Clamp(position.x, minX, maxX);
        position.y = Mathf.Clamp(position.y, minY, maxY);

        return position;
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

        if (createdTargetPos && targetPos != null)
        {
            Destroy(targetPos.gameObject);
        }

        Destroy(gameObject);
    }
}
