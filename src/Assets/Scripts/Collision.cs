using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class Collision : MonoBehaviour
{
    public GameObject gameObject;

    void OnTriggerEnter2D(Collider2D other)
    {
        // Если столкнулся с нужным объектом (например, с тегом "Player"), уничтожь себя
        if (other.CompareTag("Player"))
        {
            var spawnedObject = gameObject.GetComponent<SpawnedObject>();
            var player = other.GetComponent<PlayerManager>();
            var stopGame = player.SetValue(spawnedObject);
            if (!stopGame)
            {
                if (spawnedObject.ExpressionType == ExpressionTypes.Subtraction)
                {
                    var audioSource = other.GetComponents<AudioSource>()[1];
                    audioSource.time = 0.5f;
                    audioSource.Play();
                }
                else
                {
                    var objProperties = spawnedObject.GetComponent<PositiveModel>();
                    var positiveObs = FindObjectsOfType<PositiveModel>();
                    if (objProperties != null && positiveObs != null)
                    {
                        var audioSource = other.GetComponents<AudioSource>()[0];
                        audioSource.time = 0.3f;
                        audioSource.Play();

                        var pairObject = positiveObs
                            .Where(x => x.PairId == objProperties.PairId)
                            .Where(x => x.Id != objProperties.Id)
                            .SingleOrDefault();

                        if (pairObject != null)
                        {
                            CoroutineManager.Instance.StartManagedCoroutine(FadeToTransparent(pairObject.gameObject));
                        }
                    }
                }

                CoroutineManager.Instance.StartManagedCoroutine(FadeToGray(spawnedObject.gameObject));
            }
        }
    }

    private IEnumerator FadeToGray(GameObject gameObject)
    {
        var img = gameObject.GetComponent<Image>();

        Color grayColor = Color.gray; // Цвет в который изменить
        Color startColor = img.color;
        Color endColor = grayColor;
        endColor.a = 0; // Полная прозрачность

        float time = 0f;

        var animationDuration = 0.3f;
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

    private IEnumerator FadeToTransparent(GameObject gameObject)
    {
        gameObject.GetComponent<Collider2D>().isTrigger = false; 
        var img = gameObject.GetComponent<Image>();

        Color startColor = img.color;

        float time = 0f;

        var animationDuration = 0.2f;
        while (time < animationDuration)
        {
            float t = time / animationDuration;
            // Плавное смешивание цвета к серому и уменьшение альфы
            Color lerpedColor = Color.Lerp(startColor, startColor, t);
            lerpedColor.a = Mathf.Lerp(startColor.a, 0, t);
            img.color = lerpedColor;
            time += Time.deltaTime;
            yield return null;
        }

        // Устанавливаем окончательный цвет
        Color finishColor = startColor;
        finishColor.a = 0;

        // Ждем чуть-чуть, чтобы убедиться, что визуально все плавно исчезло
        yield return new WaitForSeconds(animationDuration);

        // Уничтожаем объект
        Destroy(gameObject);
    }
}