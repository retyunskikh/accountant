using UnityEngine;
using TMPro;
using System.Linq.Expressions;

public class MathStripeSpawner : MonoBehaviour
{
    public GameObject stripePrefab; // Префаб полоски с надписью
    public Canvas canvas;           // Ваш Canvas для UI объектов

    private float spawnInterval = 2f;
    private float moveDuration = 5f;

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

        // Математическая формула
        TMP_Text label = stripe.GetComponentInChildren<TMP_Text>();
        label.text = RandomMathExpression(expressionType);

        // Запуск движения вниз
        stripe.AddComponent<MoveAndDestroy>().Init(moveDuration, -canvas.pixelRect.height - height);
    }

    string RandomMathExpression(ExpressionTypes expressionType)
    {
        int a = Random.Range(1, 10);
        if (expressionType == ExpressionTypes.Addition)
            return $"+ {a*10+ Random.Range(1, 10)}";
        else
            return $"× {a}";
    }
}

// Скрипт для движения полоски вниз и уничтожения по завершении
public class MoveAndDestroy : MonoBehaviour
{
    private float duration;
    private float deltaY;
    private RectTransform rectTransform;
    private float elapsed = 0f;

    public void Init(float duration, float deltaY)
    {
        this.duration = duration;
        this.deltaY = deltaY;
        rectTransform = GetComponent<RectTransform>();
    }

    void Update()
    {
        if (rectTransform == null) return;

        elapsed += Time.deltaTime;
        float t = elapsed / duration;
        if (t < 1f)
        {
            rectTransform.anchoredPosition += new Vector2(0, deltaY * Time.deltaTime / duration);
        }
        else
        {
            Destroy(gameObject);
        }
    }
}