using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    private static int money = 10;
    public static int stockPlateCount = 0;
    private int plateCost = 1;
    [SerializeField] private int cleanPlateMoneyReward = 10;
    private WashPlate currentWashPlate;
    [SerializeField] private TextMeshProUGUI moneyText;
    [SerializeField] private TextMeshProUGUI stockText;
    [SerializeField] private GameObject washingPanel;
    [SerializeField] private GameObject platePrefab;//te-buru no ue no sara
    [SerializeField] private GameObject washPlatePrefab;//arau toki no sara\
    [SerializeField] private Transform washPoint;

    [SerializeField] private Transform plateSpawnMinPoint;
    [SerializeField] private Transform plateSpawnMaxPoint;
    [SerializeField] private float plateSlideDuration = 0.5f;
    [SerializeField] private float plateStartYOffset = 5f;

    [SerializeField] private RectTransform spongeCursor;
    [SerializeField] private Canvas canvas;
    
    [SerializeField] private Button upgradeButton;
    [SerializeField] private TextMeshProUGUI costText;
    private static int currentLevel = 0;
    private readonly float[] sizeStages = new float[] {2.0f, 3.0f, 4.0f};
    private readonly int[] costStages = new int[] {20, 30, 50};
    // private readonly float[] sizeStages = new float[] {1.1f, 1.2f, 1.3f, 1.4f, 1.5f, 1.8f, 2.2f, 2.7f, 3.3f, 4.0f};
    // private readonly int[] costStages = new int[] {20, 30, 50, 80, 130, 210, 340, 550, 890, 1440};

    void Start()
    {
        Cursor.visible = false;
        Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
        spongeCursor.gameObject.SetActive(true);
        washingPanel.SetActive(false);
        UpdateMoneyText();
        UpdateStockText();
        UpdateUpgradeButton();
    }

    // Update is called once per frame
    void Update()
    {
        MoveSpongeCursor();
    }

    void UpdateMoneyText()
    {
        moneyText.text = "所持金 : " + money + "円"; //.Tostring()は書かなくても勝手に解釈されるらしい。
    }

    void UpdateStockText()
    {
        if (stockText == null)
        {
            return;
        }

        stockText.text = "Stock : " + stockPlateCount + "枚";
    }

    public void AddMoney(int amount)
    {
        money += amount;
        UpdateMoneyText();
    }

    public void AddCleanPlateToStock()
    {
        stockPlateCount++;
        UpdateStockText();
    }

    public void SellCleanPlate()
    {
        AddMoney(cleanPlateMoneyReward);
    }

    public void StartWashing(Plate tablePlate)
    {
        if (currentWashPlate != null)
        {
            currentWashPlate.gameObject.SetActive(false);
        }

        WashPlate washPlate = tablePlate.GetWashPlate();

        if(washPlate == null)
        {
            washingPanel.SetActive(true);
            GameObject washPlateObject = Instantiate(washPlatePrefab, washPoint.position, Quaternion.identity);
            washPlate = washPlateObject.GetComponent<WashPlate>();
            washPlate.SetSourcePlate(tablePlate);
            tablePlate.SetWashPlate(washPlate);
        }

        washPlate.transform.position = washPoint.position;
        washPlate.transform.localScale = new Vector3(3f, 3f, 1f);
        washPlate.gameObject.SetActive(true);

        currentWashPlate = washPlate;
    }
    public void CloseWashing()
    {
        if (currentWashPlate == null)
        {
            return;
        }

        washingPanel.SetActive(false);
        currentWashPlate.gameObject.SetActive(false);
        currentWashPlate = null;
    }

    public void OnClickSpawnPlateButton()
    {
        if (money < plateCost)
        {
            money -= 5;
            UpdateMoneyText();
            SpawnPlate();
            return;
        }
        money -= plateCost;
        UpdateMoneyText();

        SpawnPlate();
    }

    void SpawnPlate()
    {
        float randomX = (Random.Range(plateSpawnMinPoint.position.x, plateSpawnMaxPoint.position.x) + Random.Range(plateSpawnMinPoint.position.x, plateSpawnMaxPoint.position.x)) / 2f;
        float randomY = (Random.Range(plateSpawnMinPoint.position.y, plateSpawnMaxPoint.position.y) + Random.Range(plateSpawnMinPoint.position.y, plateSpawnMaxPoint.position.y)) / 2f;
        Vector3 spawnPosition = new Vector3(randomX, randomY, plateSpawnMinPoint.position.z);
        Vector3 startPosition = new Vector3(spawnPosition.x * 0.5f, spawnPosition.y + plateStartYOffset, spawnPosition.z);
        GameObject plate = Instantiate(platePrefab, startPosition, Quaternion.identity);
        StartCoroutine(SlidePlate(plate.transform, startPosition, spawnPosition));
        IEnumerator SlidePlate(Transform plateTransform, Vector3 startPos, Vector3 endPos)
        {
            float elapsedTime = 0f;
            while (elapsedTime < plateSlideDuration)
            {
                plateTransform.position = Vector3.Lerp(startPos, endPos, elapsedTime / plateSlideDuration);
                elapsedTime += Time.deltaTime;
                yield return null;
            }
            plateTransform.position = endPos;
        }
    }

    public void FinishWashing()
    {
        washingPanel.SetActive(false);
        currentWashPlate = null;
    }

    public bool IsWashingOpen()
    {
        return currentWashPlate != null;
    }

    public void GoToBattleScene()
    {
        Cursor.visible = true;
        spongeCursor.gameObject.SetActive(false);
        SceneManager.LoadScene("Battle");
    }

    void MoveSpongeCursor()
    {
        if (spongeCursor == null)
        {
            return;
        }
        RectTransformUtility.ScreenPointToLocalPointInRectangle(canvas.transform as RectTransform, Input.mousePosition, null, out Vector2 localPoint);
        spongeCursor.localPosition = localPoint;
    }
    
    public void UpdateUpgradeButton()
    {
        if (currentLevel >= sizeStages.Length)
        {
            return;
        }

        int needCost = costStages[currentLevel];

        if (money >= needCost)
        {
            upgradeButton.interactable = true;
            money -= needCost;
            float newSize = sizeStages[currentLevel];
            spongeCursor.localScale = new Vector3(newSize, newSize, 1f);
            currentLevel++;
            UpdateUpgradeUI();
            UpdateMoneyText();
        }
    }

    private void UpdateUpgradeUI()
    {
        if (currentLevel >= costStages.Length)
        {
            costText.text = "MAX";
            upgradeButton.interactable = false;
            return;
        }

        int needCost = costStages[currentLevel];
        costText.text = needCost.ToString() + "円";
    }
}
