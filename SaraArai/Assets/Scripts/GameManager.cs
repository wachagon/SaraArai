using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    private int money = 10;
    private int plateCost = 1;
    private GameObject currentWashPlate;
    [SerializeField] private TextMeshProUGUI moneyText;
    [SerializeField] private GameObject platePrefab;//te-buru no ue no sara
    [SerializeField] private GameObject washPlatePrefab;//arau toki no sara\
    [SerializeField] private Transform washPoint;

    [SerializeField] private Transform plateSpawnMinPoint;
    [SerializeField] private Transform plateSpawnMaxPoint;
    [SerializeField] private float plateSlideDuration = 0.5f;
    [SerializeField] private float plateStartYOffset = 5f;

    // void Awake()
    // {
    //     moneyText = GameObject.Find("MoneyText").GetComponent<TextMeshProUGUI>();
    // }
    // Start is called before the first frame update
    void Start()
    {
        UpdateMoneyText();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void UpdateMoneyText()
    {
        moneyText.text = "所持金 : " + money + "円"; //.Tostring()は書かなくても勝手に解釈されるらしい。
    }
    public void AddMoney(int amount)
    {
        money += amount;
        UpdateMoneyText();
    }

    public void StartWashing(Plate tablePlate)
    {
        if (currentWashPlate != null)
        {
            return;
        }
        currentWashPlate = Instantiate(washPlatePrefab, washPoint.position, Quaternion.identity);
        currentWashPlate.transform.localScale = new Vector3(3f, 3f, 1f);
    }

    public void OnClickSpawnPlateButton()
    {
        if (money < plateCost)
        {
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
        AddMoney(10);
        currentWashPlate = null;
    }
}
