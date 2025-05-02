using System.Collections;
using TMPro;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    public Vector2 leftPos;
    public Vector2 rightPos;
    public bool isPortrait = Screen.width <= Screen.height;
    private bool isMoving = false;
    private float moveDuration = 0.3f; // Время на весь переход
    public int mass = PlayerStartMass.Value; // Начальная масса игрока

    public SpriteRenderer spriteRenderer;
    private Color ghostColor = new Color(100f/255, 100f/255, 100f/255, 0.5f); // полупрозрачный
    public float ghostFadeInTime = 0.2f; // Время появления призрака

    private GameObject gameOverObj;
    private GameObject playFieldObj;

    public bool SetValue(SpawnedObject spawnedObject)
    {
        if (spawnedObject.ExpressionType == ExpressionTypes.Addition)
        {
            mass += spawnedObject.Value;
        }
        if (spawnedObject.ExpressionType == ExpressionTypes.Multiplication)
        {
            mass *= spawnedObject.Value;
        }
        if (spawnedObject.ExpressionType == ExpressionTypes.Subtraction)
        {
            mass -= spawnedObject.Value;
        }

        transform.GetComponent<PlayerScaleAndColor>().PlayAnimation(spawnedObject.ExpressionType);

        var textComponent = transform.Find("PlayerMass").GetComponent<TMP_Text>();
        textComponent.text = mass.ToString();

        var gameOver = gameOverObj.GetComponent<GameOver>();
        return gameOver.Check(mass);
    }

    void Start()
    {
        var rectTransform = GetComponent<RectTransform>();
        var canvas = GetComponentInParent<Canvas>();
        gameOverObj = GameObject.Find("GameOver");
        playFieldObj = GameObject.Find("PlayField");
        gameOverObj.SetActive(false);
        transform.Find("PlayerMass").GetComponent<TMP_Text>().text = mass.ToString();
        MoveToDefaultPosition();
    }

    public void MoveToDefaultPosition()
    {
        leftPos = new Vector2(-0.25f, -0.75f);
        rightPos = new Vector2(0.25f, -0.75f);

        CoroutineManager.Instance.StartManagedCoroutine(MoveToPosition(leftPos));
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
                    SelectPosition();
                }
            }
            else
            {
                // Проверяем тач (касание) отдельно, если нужно только на устройствах с тачем
                if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)
                {
                    Touch touch = Input.GetTouch(0);
                    if (touch.phase == TouchPhase.Began)
                    {
                        SelectPosition();
                    }
                }
            }
        }
    }

    private void SelectPosition()
    {
        if (transform.position.x <0)
            CoroutineManager.Instance.StartManagedCoroutine(MoveToPosition(rightPos));
        else 
            CoroutineManager.Instance.StartManagedCoroutine(MoveToPosition(leftPos));
    }

    public IEnumerator MoveToPosition(Vector2 destination)
    {
        isMoving = true;

        var audioSource = gameObject.GetComponents<AudioSource>()[2];
        audioSource.time = 0.1f;
        audioSource.Play();

        Vector2 start = transform.position;
        float elapsed = 0f;

        // --- Создание призрака ---
        GameObject ghost = new GameObject("Ghost");
        SpriteRenderer ghostRenderer = ghost.AddComponent<SpriteRenderer>();
        ghostRenderer.sprite = spriteRenderer.sprite;
        ghostRenderer.sortingOrder = spriteRenderer.sortingOrder - 1;
        ghostRenderer.color = new Color(ghostColor.r, ghostColor.g, ghostColor.b, 0f); // Начальная альфа 0
        ghost.transform.position = destination;
        if (playFieldObj != null)
            ghost.transform.parent = playFieldObj.transform;
        ghost.transform.localScale = transform.localScale;

        // --- Плавное появление призрака ---
        float ghostFadeElapsed = 0f;
        while (ghostFadeElapsed < ghostFadeInTime)
        {
            ghostFadeElapsed += Time.deltaTime;
            float alpha = Mathf.Lerp(0f, ghostColor.a, ghostFadeElapsed / ghostFadeInTime);
            ghostRenderer.color = new Color(ghostColor.r, ghostColor.g, ghostColor.b, alpha);
            yield return null;
        }
        ghostRenderer.color = ghostColor;

        // --- Движение объекта ---
        while (elapsed < moveDuration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / moveDuration);
            float smoothT = Mathf.SmoothStep(0, 1, t);
            if (transform != null)
            {
                transform.position = Vector2.Lerp(start, destination, smoothT);
            }
            yield return null;
        }
        transform.position = destination;

        // --- Удаление призрака ---
        if (ghost != null) Destroy(ghost);

        isMoving = false;
        //currentMove = null;
    }
}