using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class Plate : MonoBehaviour
{
    private GameManager gameManager;
    void Awake()
    {
        gameManager = FindObjectOfType<GameManager>();
    }

    public void OnMouseDown()
    {
        gameManager.StartWashing(this);
    }
}
