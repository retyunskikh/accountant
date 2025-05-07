using UnityEngine;
using TMPro;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;
using System;

public class SubtractorSpawner : MonoBehaviour
{
    public GameObject stripePrefab; // Префаб полоски с надписью
    public Canvas canvas;           // Ваш Canvas для UI объектов
    private PlayerManager playerManager;

    public int subtractorValue = 0;

    private float spawnIntervalDefault = 6;
    private float moveDurationDefault = 15f;
    private float spawnInterval;
    private float moveDuration;
    public float animationDuration = 4f; // Длительность анимации

    int spawnerNumber = 1;
    int spawnerShift = 0;

    private List<GameObject> spawnedStripes = new List<GameObject>();

    void Start()
    {
        spawnInterval = spawnIntervalDefault / GlobalVariables.Instance.speedScale;
        moveDuration = moveDurationDefault / GlobalVariables.Instance.speedScale;

        playerManager = FindObjectOfType<PlayerManager>();
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
            if ((spawnerNumber + spawnerShift) % 4 == 0) // Фиксируем каждый 4й
            {

                float screenW = canvas.pixelRect.width;
                float screenH = canvas.pixelRect.height;
                float stripeWidth = screenW * 0.25f; // 25% ширины на каждую
                float stripeHeight = 100f;
                float y = screenH - stripeHeight * 2; // Чуть ниже верхней границы
                var pairId = System.Guid.NewGuid(); // Уникальный идентификатор для пары

                var spawnedObjects = new List<SpawnedShortDataModel>();

                var multiplicationValue = GetValue();

                // Левая половина
                Vector2 leftPos = new Vector2(screenW * 0.25f, y);
                spawnedObjects.Add(CreateStripe(new SpawnedDetailDataModel(leftPos, stripeWidth, stripeHeight, ExpressionTypes.Subtraction, pairId, multiplicationValue)));

                // Правая половина
                Vector2 rightPos = new Vector2(screenW * 0.75f, y);
                spawnedObjects.Add(CreateStripe(new SpawnedDetailDataModel(rightPos, stripeWidth, stripeHeight, ExpressionTypes.Division, pairId, multiplicationValue)));

                HistoryManager.Instance.HistoryAdd(new PairDataModel(ExpressionTypes.AdditionAndMultiplication, spawnedObjects));
                HistoryManager.Instance.PossibleMassAdd(spawnedObjects);
            }
        }
        spawnerNumber++;
    }

    SpawnedShortDataModel CreateStripe(SpawnedDetailDataModel model)
    {
        GameObject stripe = Instantiate(stripePrefab, canvas.transform);
        spawnedStripes.Add(stripe);

        // RectTransform для позиционирования
        var rt = stripe.GetComponent<RectTransform>();
        rt.sizeDelta = new Vector2(model.Width, model.Height);
        rt.anchoredPosition = new Vector2(model.CenterPos.x - canvas.pixelRect.width / 2, model.CenterPos.y - canvas.pixelRect.height / 2);

        var spawnedObject = stripe.GetComponent<SpawnedObject>();
        spawnedObject.ExpressionType = model.ExpressionType;

        var label = stripe.GetComponentInChildren<TMP_Text>();
        if (model.ExpressionType == ExpressionTypes.Division)
        {
            spawnedObject.Value = (float)Math.Round(model.MultiplicationValue,1);
            label.text = $"/ {spawnedObject.Value}";
        }
        else
        {

        }

        // Запуск движения вниз
        stripe.AddComponent<MoveAndDestroy>().Init(moveDuration, -canvas.pixelRect.height - model.Height);
        var positiveProperties = stripe.AddComponent<PositiveObject>();
        positiveProperties.PairId = model.PairId;
        positiveProperties.Id = System.Guid.NewGuid();

        CoroutineManager.Instance.StartManagedCoroutine(SmoothRendering(stripe));
        return new SpawnedShortDataModel(spawnedObject.Value, model.ExpressionType);

    }

    IEnumerator SmoothRendering(GameObject spawnedObject)
    {
        var image = spawnedObject.GetComponent<Image>();

        float elapsed = 0f;
        var c = image.color;
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

    public void CancelInvokes()
    {
        CancelInvoke();

        // Уничтожить все созданные объекты
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

    private float GetValue()
    {
        float a = UnityEngine.Random.Range(2, playerManager.mass % 2 + 1);
        var resultValue = HistoryManager.Instance.PossibleMassGet() - a;

        var spawnedDataModel = new SpawnedShortDataModel(resultValue, ExpressionTypes.Subtraction);
        HistoryManager.Instance.HistoryAdd(new PairDataModel(ExpressionTypes.SubtractionAndDivision, new List<SpawnedShortDataModel> { spawnedDataModel }));
        HistoryManager.Instance.PossibleMassSubtraction(resultValue);

        return resultValue;
    }

    public void Acceleration()
    {
        spawnInterval = spawnIntervalDefault / GlobalVariables.Instance.speedScale;
        moveDuration = moveDurationDefault / GlobalVariables.Instance.speedScale;

        CancelInvoke(nameof(SpawnStripes));
        var delay = spawnInterval / 3;
        InvokeRepeating(nameof(SpawnStripes), delay + 1, spawnInterval);
    }
}