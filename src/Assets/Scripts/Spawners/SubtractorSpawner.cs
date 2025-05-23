﻿using UnityEngine;
using TMPro;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;

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
                float stripeHeight = 100f;

                float y = screenH - stripeHeight * 2; // Чуть ниже верхней границы

                // Левая половина
                var leftPos = new Vector2(0, y);
                CreateStripe(leftPos, screenW, stripeHeight);
            }
        }
        spawnerNumber++;
    }

    void CreateStripe(Vector2 centerPos, float width, float height)
    {
        var stripe = Instantiate(stripePrefab, canvas.transform);
        spawnedStripes.Add(stripe);

        // RectTransform для позиционирования
        var rt = stripe.GetComponent<RectTransform>();
        rt.sizeDelta = new Vector2(width, height);
        rt.anchoredPosition = new Vector2(centerPos.x, centerPos.y - canvas.pixelRect.height / 2);

        int value = GetValue();
        subtractorValue = value;

        var spawnedObject = stripe.GetComponent<SpawnedObject>();
        spawnedObject.Value = value;
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

    private int GetValue()
    {
        int a = Random.Range(2, playerManager.mass % 2 + 1);
        var resultValue = HistoryManager.Instance.PossibleMassGet() - a;

        var spawnedDataModel = new SpawnedDataModel(resultValue, ExpressionTypes.Subtraction);
        HistoryManager.Instance.HistoryAdd(new PairDataModel(ExpressionTypes.SubtractionAndDivision, new List<SpawnedDataModel> { spawnedDataModel }));
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