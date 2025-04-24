using UnityEngine;
using TMPro;

public class SubtractorSpawner : MonoBehaviour
{
    public GameObject stripePrefab; // Префаб полоски с надписью
    public Canvas canvas;           // Ваш Canvas для UI объектов
    public int mass = 1;

    private float spawnInterval = 2f;
    private float moveDuration = 3f;

    void Start()
    {
        InvokeRepeating(nameof(SpawnStripes), 3f, spawnInterval);
    }

    void SpawnStripes()
    {
        float screenW = canvas.pixelRect.width;
        float screenH = canvas.pixelRect.height;
        float stripeHeight = 100f;

        float y = screenH - stripeHeight * 2; // Чуть ниже верхней границы

        // Левая половина
        Vector2 leftPos = new Vector2(0, y);
        CreateStripe(leftPos, screenW, stripeHeight);
    }

    void CreateStripe(Vector2 centerPos, float width, float height)
    {
        GameObject stripe = Instantiate(stripePrefab, canvas.transform);

        // RectTransform для позиционирования
        RectTransform rt = stripe.GetComponent<RectTransform>();
        rt.sizeDelta = new Vector2(width, height);
        rt.anchoredPosition = new Vector2(centerPos.x, centerPos.y - canvas.pixelRect.height / 2);

        int value = GetRandomValue();
        mass += 10;

        var spawnedObject = stripe.GetComponent<SpawnedObject>();
        spawnedObject.value = value;
        spawnedObject.ExpressionType = ExpressionTypes.Subtraction;

        var label = stripe.GetComponentInChildren<TMP_Text>();
            label.text = $"- {value}";

        // Запуск движения вниз
        stripe.AddComponent<MoveAndDestroy>().Init(moveDuration, -canvas.pixelRect.height - height);
    }

    int GetRandomValue()
    {
        int a = Random.Range(1, 10);
        return a * 10 + mass + Random.Range(1, 10);
    }
}