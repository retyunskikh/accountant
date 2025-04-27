using UnityEngine;
using TMPro;
using System.Collections;
using UnityEngine.UI;

public class SubtractorSpawner : MonoBehaviour
{
    public GameObject stripePrefab; // Префаб полоски с надписью
    public Canvas canvas;           // Ваш Canvas для UI объектов
    public int mass = 1;

    private float spawnInterval = 4f;
    private float moveDuration = 10f;
    public float animationDuration = 3f; // Длительность анимации

    void Start()
    {
        InvokeRepeating(nameof(SpawnStripes), 6f, spawnInterval);
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

        StartCoroutine(SmoothRendering(stripe));
    }

    IEnumerator SmoothRendering(GameObject spawnedObject)
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

    private int GetRandomValue()
    {
        int a = Random.Range(1, 10);
        return a * 10 + mass + Random.Range(1, 10);
    }
}