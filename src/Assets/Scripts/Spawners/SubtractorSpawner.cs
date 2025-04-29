using UnityEngine;
using TMPro;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;

public class SubtractorSpawner : MonoBehaviour
{
    public GameObject stripePrefab; // Префаб полоски с надписью
    public Canvas canvas;           // Ваш Canvas для UI объектов
    public int mass = 1;

    private float spawnInterval = 4f;
    private float moveDuration = 5f;
    public float animationDuration = 3f; // Длительность анимации
    private List<GameObject> spawnedStripes = new List<GameObject>();

    void Start()
    {
        InvokeRepeating(nameof(SpawnStripes), 5f, spawnInterval);
    }

    void SpawnStripes()
    {
        float screenW = canvas.pixelRect.width;
        float screenH = canvas.pixelRect.height;
        float stripeHeight = 100f;

        float y = screenH - stripeHeight * 2; // Чуть ниже верхней границы

        // Левая половина
        var leftPos = new Vector2(0, y);
        CreateStripe(leftPos, screenW, stripeHeight);
    }

    void CreateStripe(Vector2 centerPos, float width, float height)
    {
        var stripe = Instantiate(stripePrefab, canvas.transform);
        spawnedStripes.Add(stripe);

        // RectTransform для позиционирования
        var rt = stripe.GetComponent<RectTransform>();
        rt.sizeDelta = new Vector2(width, height);
        rt.anchoredPosition = new Vector2(centerPos.x, centerPos.y - canvas.pixelRect.height / 2);

        int value = GetRandomValue()*100;
        mass += 10;

        var spawnedObject = stripe.GetComponent<SpawnedObject>();
        spawnedObject.value = value;
        spawnedObject.ExpressionType = ExpressionTypes.Subtraction;

        var label = stripe.GetComponentInChildren<TMP_Text>();
            label.text = $"- {value}";

        // Запуск движения вниз
        stripe.AddComponent<MoveAndDestroy>().Init(moveDuration, -canvas.pixelRect.height - height);

        CoroutineManager.Instance.StartManagedCoroutine(SmoothRendering(stripe));
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

    private int GetRandomValue()
    {
        int a = Random.Range(1, 10);
        return a * 10 + mass + Random.Range(1, 10);
    }
}