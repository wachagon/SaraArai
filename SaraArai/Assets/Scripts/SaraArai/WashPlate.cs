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

    [SerializeField] private float cleanPower = 0.15f;

    private float initialDirtAmount;
    private bool isCleaned = false;

    [SerializeField] private float cleanThreshold = 0.05f;

    [SerializeField] private Transform starVisual;
    private RectTransform spongeCursor;
    [SerializeField] private float baseBrushWidth = 1f;
    [SerializeField] private float baseBrushHeight = 1f;
    


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
    }
    // Start is called before the first frame update
    void Start()
    {
        GameObject spongeCursorObj = GameObject.Find("SpongeCursorImage");
        spongeCursor = spongeCursorObj.GetComponent<RectTransform>();

        Texture2D originalTexture = dirtRenderer.sprite.texture;

        dirtTexture = new Texture2D(originalTexture.width, originalTexture.height, TextureFormat.RGBA32, false);
        dirtTexture.filterMode = FilterMode.Point;
        dirtTexture.wrapMode = TextureWrapMode.Clamp;

        dirtTexture.SetPixels(originalTexture.GetPixels());
        dirtTexture.Apply();

        dirtRenderer.sprite = Sprite.Create(dirtTexture, dirtRenderer.sprite.rect, new Vector2(0.5f, 0.5f), dirtRenderer.sprite.pixelsPerUnit);

        initialDirtAmount = CalculateDirtAmount();
        starVisual.gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (!isWashing)
        {
            return;
        }

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
        float topLeftX = localPosition.x * pixelsPerUnit + rect.width / 2f;
        float topLeftY = localPosition.y * pixelsPerUnit + rect.height / 2f;
        float currentWidth = baseBrushWidth * spongeCursor.localScale.x;
        float currentHeight = baseBrushHeight * spongeCursor.localScale.y;
        float halfWidth = currentWidth / 2f;
        float halfHeight = currentHeight / 2f;

        float angleDeg = 40f;
        float angleRad = angleDeg * Mathf.Deg2Rad;
        float cos = Mathf.Cos(angleRad);
        float sin = Mathf.Sin(angleRad);
        int searchRange = Mathf.CeilToInt(Mathf.Max(currentWidth, currentHeight) * 0.71f);

        for (int y = -searchRange; y <= searchRange; y++)
        {
            for (int x = -searchRange; x <= searchRange; x++)
            {
                float rotatedX = x * cos + y * sin;
                float rotatedY = -x * sin + y * cos;
                if (rotatedX < 0 || rotatedX > currentWidth || rotatedY > 0 || rotatedY < -currentHeight)
                {
                    continue;
                }

                int pixelX = Mathf.RoundToInt(topLeftX + x);
                int pixelY = Mathf.RoundToInt(topLeftY + y);
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

            sourcePlate.SetClean();
            sourcePlate.ClearWashPlate();
            starVisual.gameObject.SetActive(true);

            gameManager.FinishWashing();
            Destroy(gameObject);
        }
    }
    public void SetSourcePlate(Plate plate)
    {
        sourcePlate = plate;
    }
}
