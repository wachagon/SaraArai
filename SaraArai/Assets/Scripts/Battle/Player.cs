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

        Vector2 shootDir = Vector2.zero;
        if (Input.GetMouseButton(0))
        {
            Vector3 mouseScreenPos = Input.mousePosition;
            mouseScreenPos.z = -Camera.main.transform.position.z;
            Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(mouseScreenPos);

            shootDir = ((Vector2)mouseWorldPos - (Vector2)transform.position).normalized;
        }

        if (shootTimer <= 0 && shootDir != Vector2.zero)
        {
            Shoot(shootDir);
            shootTimer = shootInterval;
        }
    }

    void FixedUpdate()
    {
        rb.velocity = moveInput * moveSpeed;
    }

    void Shoot(Vector2 direction)
    {
        GameObject bullet = Instantiate(bulletPrefab, transform.position, Quaternion.identity);
        Rigidbody2D bulletRb = bullet.GetComponent<Rigidbody2D>();

        if (bulletRb != null)
        {
            bulletRb.velocity = direction * bulletSpeed;
        }

        Destroy(bullet, 2f);
    }
}
