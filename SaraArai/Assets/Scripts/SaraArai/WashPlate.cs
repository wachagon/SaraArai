using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class WashPlate : MonoBehaviour
{
    private SortingGroup sortingGroup;
    [SerializeField] private SpriteRenderer dirtRenderer;

    private Texture2D dirtTexture;
    private Collider2D plateCollider;
    private GameManager gameManager;
    private bool isWashing = true;
    private Plate sourcePlate;

    [SerializeField] private int brushRadius = 3;
    [SerializeField] private float cleanPower = 0.15f;

    private float initialDirtAmount;
    private bool isCleaned = false;

    [SerializeField] private float cleanThreshold = 0.05f;

    [SerializeField] private Transform spongeVisual;


    void Awake()
    {
        sortingGroup = GetComponent<SortingGroup>();
        plateCollider = GetComponent<Collider2D>();
        gameManager = FindObjectOfType<GameManager>();
    }

    public void OnMouseDown()
    {
        GameObject washPoint = GameObject.Find("WashPoint");
        transform.position = washPoint.transform.position;
        transform.localScale = new Vector3(3f, 3f, 1f);
        sortingGroup.sortingOrder = 100;
        isWashing = true;
        spongeVisual.gameObject.SetActive(true);
    }
    // Start is called before the first frame update
    void Start()
    {
        Texture2D originalTexture = dirtRenderer.sprite.texture;

        dirtTexture = new Texture2D(originalTexture.width, originalTexture.height, TextureFormat.RGBA32, false);
        dirtTexture.filterMode = FilterMode.Point;
        dirtTexture.wrapMode = TextureWrapMode.Clamp;

        dirtTexture.SetPixels(originalTexture.GetPixels());
        dirtTexture.Apply();

        dirtRenderer.sprite = Sprite.Create(dirtTexture, dirtRenderer.sprite.rect, new Vector2(0.5f, 0.5f), dirtRenderer.sprite.pixelsPerUnit);

        initialDirtAmount = CalculateDirtAmount();
        spongeVisual.gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (!isWashing)
        {
            return;
        }

        MoveSpongeToMouse();

        if (Input.GetMouseButtonDown(0)) // 左クリックを開始した瞬間
        {
            if (!IsMouseOverPlate())
            {
                gameManager.CloseWashing();
                return;
            }
        }

        if (Input.GetMouseButton(0)) // 左クリックを押している間
        {
            CleanAtMousePosition();
        }
    }

    private void CleanAtMousePosition()
    {
        Vector3 mouseWorldPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector3 localPosition = dirtRenderer.transform.InverseTransformPoint(mouseWorldPosition);
        Sprite sprite = dirtRenderer.sprite;
        float pixelsPerUnit = sprite.pixelsPerUnit;
        Rect rect = sprite.rect;
        int centerX = Mathf.RoundToInt(localPosition.x * pixelsPerUnit + rect.width / 2f);
        int centerY = Mathf.RoundToInt(localPosition.y * pixelsPerUnit + rect.height / 2f);
        for (int y = -brushRadius; y <= brushRadius; y++)
        {
            for (int x = -brushRadius; x <= brushRadius; x++)
            {
                if (x * x + y * y > brushRadius * brushRadius)
                {
                    continue;
                }
                int pixelX = centerX + x;
                int pixelY = centerY + y;
                if (pixelX < 0 || pixelX >= dirtTexture.width || pixelY < 0 || pixelY >= dirtTexture.height)
                {
                    continue;
                }
                Color color = dirtTexture.GetPixel(pixelX, pixelY);
                color.a -= cleanPower;
                color.a = Mathf.Clamp01(color.a);
                dirtTexture.SetPixel(pixelX, pixelY, color);
            }
        }
        dirtTexture.Apply();
        CheckCleaned();
    }

    private float CalculateDirtAmount()
    {
        Color[] pixels = dirtTexture.GetPixels();
        float dirtAmount = 0f;
        foreach (Color pixel in pixels)
        {
            dirtAmount += pixel.a;
        }
        return dirtAmount;
    }

    private bool IsMouseOverPlate()
    {
        Vector3 mouseWorldPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 mousePosition2D = new Vector2(mouseWorldPosition.x, mouseWorldPosition.y);

        return plateCollider.OverlapPoint(mousePosition2D);
    }
    private void CheckCleaned()
    {
        if (isCleaned)
        {
            return;
        }
        float currentDirtAmount = CalculateDirtAmount();
        if (currentDirtAmount / initialDirtAmount <= cleanThreshold)
        {
            isCleaned = true;
            isWashing = false;
            spongeVisual.gameObject.SetActive(false);

            sourcePlate.SetClean();
            sourcePlate.ClearWashPlate();

            gameManager.FinishWashing();
            Destroy(gameObject);
        }
    }

    private void MoveSpongeToMouse()
    {
        Vector3 mouseWorldPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mouseWorldPosition.z = spongeVisual.position.z;
        spongeVisual.position = mouseWorldPosition;
    }

    public void SetSourcePlate(Plate plate)
    {
        sourcePlate = plate;
    }
}
