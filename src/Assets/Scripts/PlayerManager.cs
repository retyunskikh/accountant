using System.Collections;
using TMPro;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    public Vector2 leftPos;
    public Vector2 rightPos;
    public float verticalPos = -4f;
    public bool isPortrait = Screen.width <= Screen.height;
    private bool isMoving = false;
    public float moveDuration = 0.5f; // Время на весь переход
    public int mass = 1; // масса игрока

    public void SetValue(SpawnedObject spawnedObject)
    {
        if (spawnedObject.ExpressionType == ExpressionTypes.Addition)
        {
            mass += spawnedObject.value;
        }
        if (spawnedObject.ExpressionType == ExpressionTypes.Multiplication)
        {
            mass *= spawnedObject.value;
        }
        if (spawnedObject.ExpressionType == ExpressionTypes.Subtraction)
        {
            mass -= spawnedObject.value;
        }

        var testTransform = transform.Find("PlayerMass");
        var textComponent = testTransform.GetComponent<TMP_Text>();
        textComponent.text = mass.ToString();
    }

    void Start()
    {
        SetDefaultPosition();
    }

    void SetDefaultPosition()
    {
        var rectTransform = GetComponent<RectTransform>();
        var canvas = GetComponentInParent<Canvas>();

        float canvasWidth = canvas.GetComponent<RectTransform>().rect.width;
        float xPos = canvasWidth * 0.25f;

        float canvasHeight = canvas.GetComponent<RectTransform>().rect.height;
        float yPos = canvasHeight * 0.1f;

        rectTransform.anchoredPosition = new Vector2(xPos, yPos);

        leftPos = new Vector2(xPos, yPos);
        rightPos = new Vector2(canvasWidth-xPos, yPos);
    }

    void Update()
    {
        if (!isMoving)
        {
            // Проверяем клик мышью
            if (Input.GetMouseButtonDown(0))
            {
                if (Input.GetMouseButtonDown(0))
                {
                    float mouseX = Input.mousePosition.x;
                    float middleX = Screen.width / 2;
                    if (mouseX < middleX && transform.position != (Vector3)leftPos)
                        StartCoroutine(MoveToPosition(leftPos));
                    else if (mouseX >= middleX && transform.position != (Vector3)rightPos)
                        StartCoroutine(MoveToPosition(rightPos));
                }
            }

            // Проверяем тач (касание) отдельно, если нужно только на устройствах с тачем
            if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)
            {
                Touch touch = Input.GetTouch(0);
                float middleX = Screen.width / 2;

                if (touch.phase == TouchPhase.Began)
                {
                    if (touch.position.x < middleX && transform.position != (Vector3)leftPos)
                        StartCoroutine(MoveToPosition(leftPos));
                    else if (touch.position.x >= middleX && transform.position != (Vector3)rightPos)
                        StartCoroutine(MoveToPosition(rightPos));
                }
            }
        }
    }

    IEnumerator MoveToPosition(Vector2 destination)
    {
        isMoving = true;
        Vector2 start = transform.position;
        float elapsed = 0f;

        while (elapsed < moveDuration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / moveDuration);
            // S-образная плавная функция
            float smoothT = Mathf.SmoothStep(0, 1, t);
            transform.position = Vector2.Lerp(start, destination, smoothT);
            yield return null;
        }
        transform.position = destination;
        isMoving = false;
    }
}