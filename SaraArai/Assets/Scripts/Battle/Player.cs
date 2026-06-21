using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.UI;

public class Player : MonoBehaviour
{
    [SerializeField] private Slider playerHPSlider;
    [SerializeField] private TextMeshProUGUI stockText;
    

    public float moveSpeed = 5f;
    public float maxHp = 100f;
    public GameObject bulletPrefab;
    public float bulletSpeed = 10f; 
    public float bulletDamage = 10f;
    [SerializeField] private GameObject gameOverPanel;

    private Rigidbody2D rb;
    private Vector2 moveInput;
    private float currentHp;
    private float speedMultiplier = 1f;
    private float slowTimer;

    public float shootInterval = 0.5f;
    private float shootTimer = 0f;

    private readonly float[] sizeStages = new float[] { 2.0f, 3.0f, 4.0f };

    [SerializeField] GameObject Bullet;

    void Start()
    {
        if (GameManager.stockPlateCount == 0)
        {
            Bullet.SetActive(false);
        }
        int currentLevel = PlayerPrefs.GetInt("SpongeSizeLevel", 0);
        ApplySpongeSize(currentLevel);

        rb = GetComponent<Rigidbody2D>();
        currentHp = maxHp;
        gameOverPanel.SetActive(false);

        playerHPSlider.maxValue = maxHp;
        playerHPSlider.value = currentHp;

        UpdateStockText();
        CalculateMaxHp();
    }

    void Update()
    {
        UpdateSlow();

        float moveX = (Input.GetKey(KeyCode.D) ? 1 : 0) - (Input.GetKey(KeyCode.A) ? 1 : 0);
        float moveY = (Input.GetKey(KeyCode.W) ? 1 : 0) - (Input.GetKey(KeyCode.S) ? 1 : 0);

        moveInput = new Vector2(moveX, moveY).normalized;

        if (shootTimer > 0)
        {
            shootTimer -= Time.deltaTime;
        }

        Vector2 shootDir = Vector2.zero;
        if (Input.GetMouseButton(0) && GameManager.stockPlateCount > 0)
        {
            Vector3 mouseScreenPos = Input.mousePosition;
            mouseScreenPos.z = -Camera.main.transform.position.z;
            Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(mouseScreenPos);

            shootDir = ((Vector2)mouseWorldPos - (Vector2)transform.position).normalized;
            GameManager.stockPlateCount--;
            UpdateStockText();
            if (GameManager.stockPlateCount == 0)
            {
                Bullet.SetActive(false);
            }
        }

        if (shootTimer <= 0 && shootDir != Vector2.zero)
        {
            Shoot(shootDir);
            shootTimer = shootInterval;
        }
    }

    void FixedUpdate()
    {
        rb.velocity = moveInput * moveSpeed * speedMultiplier;
    }

    void Shoot(Vector2 direction)
    {
        GameObject bullet = Instantiate(bulletPrefab, transform.position, Quaternion.identity);
        Rigidbody2D bulletRb = bullet.GetComponent<Rigidbody2D>();
        PlayerBullet playerBullet = bullet.GetComponent<PlayerBullet>();
        Collider2D bulletCollider = bullet.GetComponent<Collider2D>();

        if (playerBullet == null)
        {
            playerBullet = bullet.AddComponent<PlayerBullet>();
        }

        if (bulletCollider == null)
        {
            CircleCollider2D circleCollider = bullet.AddComponent<CircleCollider2D>();
            circleCollider.radius = 0.5f;
            circleCollider.isTrigger = true;
        }

        playerBullet.SetDamage(bulletDamage);

        if (bulletRb != null)
        {
            bulletRb.velocity = direction * bulletSpeed;
        }

        Destroy(bullet, 2f);
    }

    void UpdateSlow()
    {
        if (slowTimer <= 0f)
        {
            speedMultiplier = 1f;
            return;
        }

        slowTimer -= Time.deltaTime;
    }

    public void ApplySlow(float multiplier, float duration)
    {
        speedMultiplier = Mathf.Min(speedMultiplier, multiplier);
        slowTimer = Mathf.Max(slowTimer, duration);
    }

    public void TakeDamage(float damage)
    {
        currentHp = Mathf.Max(0f, currentHp - damage);
        UpdatePlayerHpSlider();

        if (currentHp <= 0f)
        {
            gameOverPanel.SetActive(true);
            Time.timeScale = 0f;
        }
    }

    public void Retry()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("SaraArai");
    }

    private void ApplySpongeSize(int level)
    {
        if (level == 0)
        {
            transform.localScale = new Vector3(1f, 1f, 1f);
        }
        else
        {
            float newSize = sizeStages[level - 1];
            transform.localScale = new Vector3(newSize, newSize, 1f);
        }
    }

    private void UpdateStockText()
    {
        stockText.text = "Stock: " + GameManager.stockPlateCount;
    }

    private void UpdatePlayerHpSlider()
    {
        playerHPSlider.value = currentHp;
    }

    private void CalculateMaxHp()
    {
        int currentLevel = PlayerPrefs.GetInt("SpongeSizeLevel", 0);
        if (currentLevel == 0)
        {
            maxHp = 100f;
        }
        else
        {
            maxHp = 100f * currentLevel;
        }
        currentHp = maxHp;
        playerHPSlider.maxValue = maxHp;
        playerHPSlider.value = currentHp;
    }
}
