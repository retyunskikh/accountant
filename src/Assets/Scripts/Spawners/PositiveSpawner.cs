using UnityEngine;
using TMPro;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;

public class PositiveSpawner : MonoBehaviour
{
    public GameObject stripePrefab; // Префаб полоски с надписью
    public Canvas canvas;           // Ваш Canvas для UI объектов
    private PlayerManager playerManager;
    private List<GameObject> spawnedStripes = new List<GameObject>();
    private SubtractorSpawner subtractorSpawner;

    private float spawnIntervalDefault = 6;
    private float moveDurationDefault = 15;
    private float spawnInterval = 0;
    private float moveDuration = 0;
    public float animationDuration = 4f; // Длительность анимации

    int spawnerNumber = 1;
    int spawnerShift = 0;

    int multiplicationMaxValue = 2;
    public int lastMultiplicationValue = 1;

    void Start()
    {
        spawnInterval = spawnIntervalDefault / GlobalVariables.Instance.speedScale;
        moveDuration = moveDurationDefault / GlobalVariables.Instance.speedScale;

        playerManager = FindObjectOfType<PlayerManager>();
        subtractorSpawner = FindObjectOfType<SubtractorSpawner>();

        InvokeRepeating(nameof(SpawnStripes), 0f, spawnInterval);
    }

    void SpawnStripes()
    {
        if (spawnerNumber % 9 == 0) // Фиксируем каждый 9й
        {
            spawnerShift++;
        }
        else
        {
            if ((spawnerNumber + spawnerShift) % 4 != 0) // Пропускаем каждый 4й
            {
                float screenW = canvas.pixelRect.width;
                float screenH = canvas.pixelRect.height;
                float stripeWidth = screenW * 0.25f; // 25% ширины на каждую
                float stripeHeight = 100f;

                float y = screenH - stripeHeight * 2; // Чуть ниже верхней границы

                var pairId = System.Guid.NewGuid(); // Уникальный идентификатор для пары

                var multiplicationValue = Random.Range(2, multiplicationMaxValue + 1); // Случайный мультипликатор
                if (multiplicationMaxValue < 10)
                {
                    if (Random.Range(0, 10) < 1)
                    {
                        multiplicationMaxValue++; // Постепенно повышаем верхнюю границу случайного мультипликатора
                    }
                }

                var spawnedObjects = new List<SpawnedDataModel>();

                // Левая половина
                Vector2 leftPos = new Vector2(screenW * 0.25f, y);
                spawnedObjects.Add(CreateStripe(new PositiveDataModel(leftPos, stripeWidth, stripeHeight, ExpressionTypes.Addition, pairId, multiplicationValue)));

                // Правая половина
                Vector2 rightPos = new Vector2(screenW * 0.75f, y);
                spawnedObjects.Add(CreateStripe(new PositiveDataModel(rightPos, stripeWidth, stripeHeight, ExpressionTypes.Multiplication, pairId, multiplicationValue)));

                HistoryManager.Instance.HistoryAdd(new PairDataModel(ExpressionTypes.AdditionAndMultiplication, spawnedObjects));
                HistoryManager.Instance.PossibleMassAdd(spawnedObjects);

                lastMultiplicationValue = multiplicationValue;
            }
        }
        spawnerNumber++;
    }

    SpawnedDataModel CreateStripe(PositiveDataModel model)
    {
        GameObject stripe = Instantiate(stripePrefab, canvas.transform);
        spawnedStripes.Add(stripe);

        // RectTransform для позиционирования
        RectTransform rt = stripe.GetComponent<RectTransform>();
        rt.sizeDelta = new Vector2(model.Width, model.Height);
        rt.anchoredPosition = new Vector2(model.CenterPos.x - canvas.pixelRect.width / 2, model.CenterPos.y - canvas.pixelRect.height / 2);

        var spawnedObject = stripe.GetComponent<SpawnedObject>();
        spawnedObject.ExpressionType = model.ExpressionType;

        var label = stripe.GetComponentInChildren<TMP_Text>();
        if (model.ExpressionType == ExpressionTypes.Multiplication)
        {
            spawnedObject.Value = model.MultiplicationValue;
            label.text = $"X {model.MultiplicationValue}";

        }
        else
        {
            var lastSpawnedObj = HistoryManager.Instance.HistoryLastGet();

            var massAfterCollision = playerManager.mass;
            if (lastSpawnedObj?.Value != null && lastSpawnedObj.ExpressionType == ExpressionTypes.Subtraction)
            {
                massAfterCollision -= subtractorSpawner.subtractorValue;
            }
            else
            {
                massAfterCollision *= model.MultiplicationValue;
            }

            var growth = massAfterCollision * lastMultiplicationValue - massAfterCollision;
            var randomChange = Random.Range(1, multiplicationMaxValue+1);
            var additionValue = 0;
            var rand = Random.Range(1, 3);
            if (rand == 1)
            {
                additionValue = growth - randomChange;
                if (additionValue < 1) additionValue = 1;
            }
            else
            {
                additionValue = growth + randomChange;
            }
            spawnedObject.Value = additionValue;
            label.text = $"+ {additionValue}";
        }

        // Запуск движения вниз
        stripe.AddComponent<MoveAndDestroy>().Init(moveDuration, -canvas.pixelRect.height - model.Height);
        var positiveProperties = stripe.AddComponent<PositiveObject>();
        positiveProperties.PairId = model.PairId;
        positiveProperties.Id = System.Guid.NewGuid();

        CoroutineManager.Instance.StartManagedCoroutine(SmoothRendering(spawnedObject));
        return new SpawnedDataModel(spawnedObject.Value, model.ExpressionType);
    }

    IEnumerator SmoothRendering(SpawnedObject spawnedObject)
    {
        var image = spawnedObject.GetComponent<Image>();

        float elapsed = 0f;
        Color c = image.color;
        c.a = 0f;
        image.color = c;

        while (elapsed < animationDuration)
        {
            elapsed += Time.deltaTime;
            float alpha = Mathf.Clamp01(elapsed / animationDuration); // от 0 до 1
            c.a = alpha;
            image.color = c;
            yield return null;
        }

        c.a = 1f; // Обеспечим полную непрозрачность в конце
        image.color = c;
    }

    // Отменить генерацию и уничтожить все созданные объекты
    public void CancelInvokes()
    {
        CancelInvoke(nameof(SpawnStripes));

        foreach (var stripe in spawnedStripes)
        {
            if (stripe != null)
            {
                var image = stripe.GetComponent<Image>();
                Destroy(image);
                Destroy(stripe);
            }
        }
        spawnedStripes.Clear();
    }

    public void Acceleration()
    {
        spawnInterval = spawnIntervalDefault / GlobalVariables.Instance.speedScale;
        moveDuration = moveDurationDefault / GlobalVariables.Instance.speedScale;

        CancelInvoke(nameof(SpawnStripes));
        InvokeRepeating(nameof(SpawnStripes), 2f, spawnInterval);
    }
}