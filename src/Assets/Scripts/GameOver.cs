using TMPro;
using UnityEngine;

public class GameOver : MonoBehaviour
{
    public GameObject gameObject;
    public PositiveSpawner positiveSpawner;
    public SubtractorSpawner subtractorSpawner;

    void Start()
    {
        positiveSpawner = FindObjectOfType<PositiveSpawner>();
        subtractorSpawner = FindObjectOfType<SubtractorSpawner>();
    }

    public bool Check(int userMass)
    {
        if (userMass < 0)
        {
            //gameObject.SetActive(true);
            var textComponent = gameObject.GetComponentInChildren<TMP_Text>();
            textComponent.text = $"Ваш счёт:{Time.time * 100f}";

            Time.timeScale = 0f;
            CoroutineManager.Instance.StopAllManagedCoroutines();
            positiveSpawner.CancelInvokes();
            subtractorSpawner.CancelInvokes();
            return true;
        }
        return false;
    }
}