using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public float moveSpeed = 5f;
    public GameObject bulletPrefab;
    public float bulletSpeed = 10f; 

    private Rigidbody2D rb;
    private Vector2 moveInput;

    public float shootInterval = 0.5f;
    private float shootTimer = 0f;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        float moveX = (Input.GetKey(KeyCode.D) ? 1 : 0) - (Input.GetKey(KeyCode.A) ? 1 : 0);
        float moveY = (Input.GetKey(KeyCode.W) ? 1 : 0) - (Input.GetKey(KeyCode.S) ? 1 : 0);

        moveInput = new Vector2(moveX, moveY).normalized;

        if (shootTimer > 0)
        {
            shootTimer -= Time.deltaTime;
        }

        // 2. 十字キー（矢印キー）で弾を発射
        Vector2 shootDir = Vector2.zero;
        if (Input.GetKey(KeyCode.UpArrow)) shootDir = Vector2.up;
        if (Input.GetKey(KeyCode.DownArrow)) shootDir = Vector2.down;
        if (Input.GetKey(KeyCode.LeftArrow)) shootDir = Vector2.left;
        if (Input.GetKey(KeyCode.RightArrow)) shootDir = Vector2.right;

        if (shootTimer <= 0 && shootDir != Vector2.zero)
        {
            Shoot(shootDir);
            shootTimer = shootInterval;
        }
    }

    void FixedUpdate()
    {
        // 物理演算に基づいた滑らかな移動
        rb.velocity = moveInput * moveSpeed;
    }

    void Shoot(Vector2 direction)
    {
        // 弾を生成して速度を与える
        GameObject bullet = Instantiate(bulletPrefab, transform.position, Quaternion.identity);
        Rigidbody2D bulletRb = bullet.GetComponent<Rigidbody2D>();

        if (bulletRb != null)
        {
            bulletRb.velocity = direction * bulletSpeed;
        }

        // 2秒後に自動削除
        Destroy(bullet, 2f);
    }
}
