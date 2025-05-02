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

    private float spawnInterval = 6f;
    private float moveDuration = 15f;
    public float animationDuration = 4f; // Длительность анимации

    int count = 0;
    int multiplicationMaxValue = 4;
    public int lastMultiplicationValue = 1;

    void Start()
    {
        playerManager = FindObjectOfType<PlayerManager>();
        subtractorSpawner = FindObjectOfType<SubtractorSpawner>();
        InvokeRepeating(nameof(SpawnStripes), 0f, spawnInterval);
    }

    void SpawnStripes()
    {
        count++;
        if (count == 1 || count % 3 != 1)
        {
            float screenW = canvas.pixelRect.width;
            float screenH = canvas.pixelRect.height;
            float stripeWidth = screenW * 0.25f; // 25% ширины на каждую
            float stripeHeight = 100f;

            float y = screenH - stripeHeight * 2; // Чуть ниже верхней границы

            //var expressionRand = Random.Range(0, 1);

            var pairId = System.Guid.NewGuid(); // Уникальный идентификатор для пары

            var multiplicationValue = Random.Range(2, multiplicationMaxValue); // Случайный мультипликатор
            HistoryManager.Instance.AddHistory(new SpawnedObject(multiplicationValue, ExpressionTypes.Multiplication));
            if (multiplicationMaxValue < 10)
            {
                if (Random.Range(0, 10)<1)
                {
                    multiplicationMaxValue++; // Постепенно повышаем верхнюю границу случайного мультипликатора
                }
            }

            var spawnedObjects = new List<SpawnedObject>();

            // Левая половина
            Vector2 leftPos = new Vector2(screenW * 0.25f, y);
            spawnedObjects.Add(CreateStripe(new PositiveModel(leftPos, stripeWidth, stripeHeight, ExpressionTypes.Addition, pairId, multiplicationValue)));

            // Правая половина
            Vector2 rightPos = new Vector2(screenW * 0.75f, y);
            spawnedObjects.Add(CreateStripe(new PositiveModel(rightPos, stripeWidth, stripeHeight, ExpressionTypes.Multiplication, pairId, multiplicationValue)));

            HistoryManager.Instance.PossibleMassAdd(spawnedObjects);

            lastMultiplicationValue = multiplicationValue;
        }
    }

    SpawnedObject CreateStripe(PositiveModel model)
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
            var massAfterMultiplication = playerManager.mass;
            if (count % 3 == 2)
            {
                massAfterMultiplication -= subtractorSpawner.subtractorValue; 
            }else
            {
                massAfterMultiplication *= model.MultiplicationValue * lastMultiplicationValue;
            }

            var growth = massAfterMultiplication - playerManager.mass;
            var randomChange = Random.Range(1, multiplicationMaxValue);
            var additionValue = 0;
            if (Random.Range(0, 1) == 0)
            {
                additionValue = growth - randomChange;
                if (additionValue < 1) additionValue = Random.Range(1, 3);
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
        var positiveProperties = stripe.AddComponent<PositiveModel>();
        positiveProperties.PairId = model.PairId;
        positiveProperties.Id = System.Guid.NewGuid();

        CoroutineManager.Instance.StartManagedCoroutine(SmoothRendering(spawnedObject));
        return spawnedObject;
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

    //private int GetRandomValue(ExpressionTypes expressionType)
    //{
    //    int a = Random.Range(1, 10);
    //    if (expressionType == ExpressionTypes.Addition)
    //        return a * 10 + Random.Range(1, 10);
    //    else
    //        return a;
    //}
}