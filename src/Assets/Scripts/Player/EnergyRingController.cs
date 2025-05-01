using UnityEngine;

public class EnergyRingController : MonoBehaviour
{
    SpriteRenderer sr;
    Color startColor, targetColor;
    float startSize, targetSize;
    float t = 1f;
    float transitionDuration = 1f;

    void Start()
    {
        sr = GetComponent<SpriteRenderer>();
        startColor = sr.color;
        targetColor = startColor;
        startSize = transform.localScale.x;
        targetSize = startSize;
    }

    void Update()
    {
        // Плавное изменение цвета и размера
        if (t < 1f)
        {
            t += Time.deltaTime / transitionDuration;
            sr.color = Color.Lerp(startColor, targetColor, t);
            float size = Mathf.Lerp(startSize, targetSize, t);
            transform.localScale = new Vector3(size, size, 1);
        }
        // По клику меняем цели
        if (Input.GetMouseButtonDown(0))
        {
            startColor = sr.color;
            targetColor = Random.ColorHSV(0f, 1f, 0.7f, 1f, 0.7f, 1f); // яркие цвета
            startSize = transform.localScale.x;
            t = 0f;
        }
    }
}