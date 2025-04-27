using UnityEngine;
using TMPro;
using System.Collections;
using UnityEngine.UI;

public class PositiveSpawner : MonoBehaviour
{
    public GameObject stripePrefab; // Префаб полоски с надписью
    public Canvas canvas;           // Ваш Canvas для UI объектов

    private float spawnInterval = 4f;
    private float moveDuration = 10f;
    public float animationDuration = 3f; // Длительность анимации

    void Start()
    {
        InvokeRepeating(nameof(SpawnStripes), 0f, spawnInterval);
    }

    void SpawnStripes()
    {
        float screenW = canvas.pixelRect.width;
        float screenH = canvas.pixelRect.height;
        float stripeWidth = screenW * 0.25f; // 25% ширины на каждую
        float stripeHeight = 100f;

        float y = screenH - stripeHeight * 2; // Чуть ниже верхней границы

        int expressionRand = Random.Range(0, 1);

        // Левая половина
        Vector2 leftPos = new Vector2(screenW * 0.25f, y);
        CreateStripe(leftPos, stripeWidth, stripeHeight, expressionRand==0? ExpressionTypes.Addition: ExpressionTypes.Multiplication);

        // Правая половина
        Vector2 rightPos = new Vector2(screenW * 0.75f, y);
        CreateStripe(rightPos, stripeWidth, stripeHeight, expressionRand == 0 ? ExpressionTypes.Multiplication : ExpressionTypes.Addition);
    }

    void CreateStripe(Vector2 centerPos, float width, float height, ExpressionTypes expressionType)
    {
        GameObject stripe = Instantiate(stripePrefab, canvas.transform);

        // RectTransform для позиционирования
        RectTransform rt = stripe.GetComponent<RectTransform>();
        rt.sizeDelta = new Vector2(width, height);
        rt.anchoredPosition = new Vector2(centerPos.x - canvas.pixelRect.width / 2, centerPos.y - canvas.pixelRect.height / 2);

        int value = GetRandomValue(expressionType);

        var spawnedObject = stripe.GetComponent<SpawnedObject>();
        spawnedObject.value = value;
        spawnedObject.ExpressionType = expressionType;

        var label = stripe.GetComponentInChildren<TMP_Text>();
        if (expressionType == ExpressionTypes.Addition)
        {
            label.text = $"+ {value}";
        }
        else
        {
            label.text = $"X {value}";
        }

        // Запуск движения вниз
        stripe.AddComponent<MoveAndDestroy>().Init(moveDuration, -canvas.pixelRect.height - height);

        StartCoroutine(SmoothRendering(spawnedObject));
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

    private int GetRandomValue(ExpressionTypes expressionType)
    {
        int a = Random.Range(1, 10);
        if (expressionType == ExpressionTypes.Addition)
            return a * 10 + Random.Range(1, 10);
        else
            return a;
    }
}