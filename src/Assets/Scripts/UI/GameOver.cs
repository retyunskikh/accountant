using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOver : MonoBehaviour
{
    public GameObject gameObject;
    public bool gameOver = false;

    private PlayerManager playerManager;
    private PositiveSpawner positiveSpawner;
    private SubtractorSpawner subtractorSpawner;

    void Start()
    {
        playerManager = FindObjectOfType<PlayerManager>();
        positiveSpawner = FindObjectOfType<PositiveSpawner>();
        subtractorSpawner = FindObjectOfType<SubtractorSpawner>();
    }

    public bool Check(int userMass)
    {
        if (userMass < 0)
        {
            gameOver = true;
            gameObject.SetActive(true);
            var textComponent = gameObject.GetComponentInChildren<TMP_Text>();
            textComponent.text = $"Ваш счёт:{Time.time * 100f}";

            Time.timeScale = 0f;
            CoroutineManager.Instance.StopAllManagedCoroutines();
            HistoryManager.Instance.Clear();
            positiveSpawner.CancelInvokes();
            subtractorSpawner.CancelInvokes();
            GlobalVariables.Instance.speedScale = 1f;
            return true;
        }
        return false;
    }

    void Update()
    {
        if (gameOver)
        {
            // Проверяем клик мышью
            if (Input.GetMouseButtonDown(0))
            {
                RestartGame();
            }

            // Проверяем тач (касание) отдельно, если нужно только на устройствах с тачем
            if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)
            {
                RestartGame();
            }
        }
    }

    public void RestartGame()
    {
        playerManager.SetDefaultPosition();
        gameOver = false;
        gameObject.SetActive(false);
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}