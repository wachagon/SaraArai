using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;

public class Plate : MonoBehaviour
{
    private WashPlate washPlate;
    private GameManager gameManager;
    private Vector3 dragOffset;
    private Vector3 mouseDownPosition;
    private bool isDragging = false;
    private bool isClean = false;

    [SerializeField] private float clickTimeLimit = 0.2f;
    [SerializeField] private float dragStartDistance = 0.2f;
    [SerializeField] private GameObject dirtVisual;
    [SerializeField] private string stockTargetName = "Stock";
    [SerializeField] private string moneyTargetName = "Money";
    private float mouseDownTime;

    void Awake()
    {
        gameManager = FindObjectOfType<GameManager>();
    }

    public void OnMouseDown()
    {
        if(gameManager.IsWashingOpen())
        {
            return;
        }

        mouseDownTime = Time.time;
        mouseDownPosition = GetMouseWorldPosition();
        dragOffset = transform.position - mouseDownPosition;
        isDragging = false;
    }

    private void OnMouseDrag()
    {
        if(gameManager.IsWashingOpen())
        {
            return;
        }
        
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
        if(gameManager.IsWashingOpen())
        {
            return;
        }

        if (isClean)
        {
            TryDropCleanPlate();
            isDragging = false;
            return;
        }
        
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

    public WashPlate GetWashPlate()
    {
        return washPlate;
    }

    public void SetWashPlate(WashPlate newWashPlate)
    {
        washPlate = newWashPlate;
    }

    public void SetClean()
    {
        if (dirtVisual != null)
        {
            dirtVisual.SetActive(false);
        }
        isClean = true;
    }

    public void ClearWashPlate()
    {
        washPlate = null;
    }

    private void TryDropCleanPlate()
    {
        Collider2D dropTarget = FindDropTarget();
        if (dropTarget == null)
        {
            return;
        }

        if (dropTarget.name == stockTargetName)
        {
            gameManager.AddCleanPlateToStock();
            Destroy(gameObject);
            return;
        }

        if (dropTarget.name == moneyTargetName)
        {
            gameManager.SellCleanPlate();
            Destroy(gameObject);
        }
    }

    private Collider2D FindDropTarget()
    {
        Collider2D dropTarget = FindDropTargetAtPoint(GetMouseWorldPosition());
        if (dropTarget != null)
        {
            return dropTarget;
        }

        return FindDropTargetAtPoint(transform.position);
    }

    private Collider2D FindDropTargetAtPoint(Vector3 position)
    {
        Collider2D[] colliders = Physics2D.OverlapPointAll(position);
        foreach (Collider2D collider in colliders)
        {
            if (collider.name == stockTargetName || collider.name == moneyTargetName)
            {
                return collider;
            }
        }

        return null;
    }
}
