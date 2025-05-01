using UnityEngine;
using System.Collections;

public class PlayerScaleAndColor : MonoBehaviour
{
    private float scaleFactor = 1.15f; // Увеличиваем / уменьшаем на 30%
    private float totalDuration = 0.3f;

    private Vector3 originalScale;
    private Color originalColor;
    private SpriteRenderer sr;

    private Color targetGreen = new Color(41f / 255f, 160f / 255f, 48f / 255f, 1f);
    private Color targetRed = new Color(229f / 255f, 17f / 255f, 48f / 255f, 1f);

    void Start()
    {
        sr = GetComponent<SpriteRenderer>();
        if (sr == null)
        {
            Debug.LogError("SpriteRenderer не найден!");
            return;
        }
        originalScale = transform.localScale;
        originalColor = sr.color;
    }

    public void PlayAnimation(ExpressionTypes expressionType)
    {
        if (expressionType == ExpressionTypes.Addition || expressionType == ExpressionTypes.Multiplication)
        {
            StartCoroutine(AnimateAddition());
        }
        else
        {
            StartCoroutine(AnimateSubtraction());
        }
    }

    IEnumerator AnimateAddition()
    {
        // Одновременно увеличиваем размер и меняем цвет за colorChangeDuration
        Vector3 targetScale = originalScale * scaleFactor;

        float t = 0f;
        while (t < totalDuration)
        {
            t += Time.deltaTime;
            float progress = Mathf.Clamp01(t / totalDuration);

            // Меняем цвет и масштаб одновременно
            sr.color = Color.Lerp(originalColor, targetGreen, progress);
            transform.localScale = Vector3.Lerp(originalScale, targetScale, progress);

            yield return null;
        }
        sr.color = targetGreen;
        transform.localScale = targetScale;

        t = 0f;
        while (t < totalDuration)
        {
            t += Time.deltaTime;
            float progress = Mathf.Clamp01(t / totalDuration);

            sr.color = Color.Lerp(targetGreen, originalColor, progress);
            transform.localScale = Vector3.Lerp(targetScale, originalScale, progress);

            yield return null;
        }
        sr.color = originalColor;
        transform.localScale = originalScale;
    }

    IEnumerator AnimateSubtraction()
    {
        // Одновременно увеличиваем размер и меняем цвет за colorChangeDuration
        Vector3 targetScale = originalScale / scaleFactor;

        float t = 0f;
        while (t < totalDuration)
        {
            t += Time.deltaTime;
            float progress = Mathf.Clamp01(t / totalDuration);

            // Меняем цвет и масштаб одновременно
            sr.color = Color.Lerp(originalColor, targetRed, progress);
            transform.localScale = Vector3.Lerp(originalScale, targetScale, progress);

            yield return null;
        }
        sr.color = targetRed;
        transform.localScale = targetScale;

        t = 0f;
        while (t < totalDuration)
        {
            t += Time.deltaTime;
            float progress = Mathf.Clamp01(t / totalDuration);

            sr.color = Color.Lerp(targetRed, originalColor, progress);
            transform.localScale = Vector3.Lerp(targetScale, originalScale, progress);

            yield return null;
        }
        sr.color = originalColor;
        transform.localScale = originalScale;
    }
}