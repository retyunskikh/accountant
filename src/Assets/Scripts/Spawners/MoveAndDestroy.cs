
// Скрипт для движения полоски вниз и уничтожения по завершении
using UnityEngine;

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