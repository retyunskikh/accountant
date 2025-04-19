using System.Collections;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public Vector2 leftPos = new Vector2(-3.5f, -4f);
    public Vector2 rightPos = new Vector2(3.5f, -4f);
    public float moveDuration = 0.5f; // Время на весь переход

    private Vector2 targetPos;
    private bool isMoving = false;

    void Start()
    {
        targetPos = leftPos;
        transform.position = leftPos;
    }

    void Update()
    {
        if (!isMoving)
        {
            // Тач на телефоне
            if (Input.touchCount > 0)
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

            // Проверка для ПК (мышка)
            if (Application.isEditor)
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