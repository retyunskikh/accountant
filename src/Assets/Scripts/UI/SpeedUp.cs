using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SpeedUp : MonoBehaviour
{
    private float duration = 1.5f;
    private float maxOpacity = 0.8f;
    public GameObject speedUp;

    public void ShowSpeedUp()
    {
        StartCoroutine(FadeInAndOut());
    }

    private IEnumerator FadeInAndOut()
    {
        yield return new WaitForSeconds(2 / GlobalVariables.Instance.speedScale);

        TMP_Text text = speedUp.GetComponentInChildren<TMP_Text>();
        text.text = $"{GlobalVariables.Instance.currentLevel.ToString()}";

        Image img = speedUp.GetComponent<Image>();
        // Fade In
        float elapsed = 0f;
        Color color = img.color;
        color.a = 0;
        img.color = color;
        while (elapsed < duration * 0.5f)
        {
            color.a = Mathf.Lerp(0, maxOpacity, elapsed / (duration * 0.5f));
            img.color = color;
            elapsed += Time.deltaTime;
            yield return null;
        }
        color.a = maxOpacity;

        // ѕо желанию: подождать немного на полной €ркости
        //yield return new WaitForSeconds(0.1f);

        // Fade Out
        elapsed = 0f;
        while (elapsed < duration)
        {
            color.a = Mathf.Lerp(maxOpacity, 0, elapsed / duration);
            img.color = color;
            elapsed += Time.deltaTime;
            yield return null;
        }
        color.a = 0;
        text.text = "";
    }
}