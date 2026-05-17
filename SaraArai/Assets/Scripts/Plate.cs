using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;

public class Plate : MonoBehaviour
{
    private GameManager gameManager;
    private Vector3 dragOffset;
    private Vector3 mouseDownPosition;
    private bool isDragging = false;
    
    [SerializeField] private float clickTimeLimit = 0.2f;
    [SerializeField] private float dragStartDistance = 0.2f;
    private float mouseDownTime;

    void Awake()
    {
        gameManager = FindObjectOfType<GameManager>();
    }

    public void OnMouseDown()
    {
        mouseDownTime = Time.time;
        mouseDownPosition = GetMouseWorldPosition();
        dragOffset = transform.position - mouseDownPosition;
        isDragging = false;
    }

    private void OnMouseDrag()
    {
        Vector3 mousePosition = GetMouseWorldPosition();

        float distance = Vector3.Distance(mouseDownPosition, mousePosition);

        if (distance >= dragStartDistance)
        {
            isDragging = true;
        }

        if (isDragging)
        {
            transform.position = mousePosition + dragOffset;
        }
    }

    private void OnMouseUp()
    {
        float holdTime = Time.time - mouseDownTime;

        if (!isDragging && holdTime <= clickTimeLimit)
        {
            gameManager.StartWashing(this);
        }
        
        isDragging = false;
    }

    private Vector3 GetMouseWorldPosition()
    {
        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePosition.z = transform.position.z;
        return mousePosition;
    }
}
