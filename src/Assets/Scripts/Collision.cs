using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class Collision : MonoBehaviour
{
    public GameObject gameObject;

    public Color grayColor = Color.gray; // Цвет в который изменить
    public float animationDuration = 0.1f; // Длительность анимации

    private Image img;

    void Start()
    {
        img = GetComponent<Image>();
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        // Если столкнулся с нужным объектом (например, с тегом "Player"), уничтожь себя
        if (other.CompareTag("Player"))
        {
            var spawnedObject = gameObject.GetComponent<SpawnedObject>();
            var player = other.GetComponent<PlayerManager>();
            player.SetValue(spawnedObject);

            StartCoroutine(FadeToGray());
        }
    }

    private IEnumerator FadeToGray()
    {
        Color startColor = img.color;
        Color endColor = grayColor;
        endColor.a = 0; // Полная прозрачность

        float time = 0f;

        while (time < animationDuration)
        {
            float t = time / animationDuration;
            // Плавное смешивание цвета к серому и уменьшение альфы
            Color lerpedColor = Color.Lerp(startColor, grayColor, t);
            //lerpedColor.a = Mathf.Lerp(startColor.a, 0, t);
            img.color = lerpedColor;
            time += Time.deltaTime;
            yield return null;
        }

        // Устанавливаем окончательный цвет
        Color finishColor = grayColor;
       // finishColor.a = 0;
        img.color = finishColor;

        // Ждем чуть-чуть, чтобы убедиться, что визуально все плавно исчезло
        yield return new WaitForSeconds(3f);

        // Уничтожаем объект
        Destroy(gameObject);
    }
}