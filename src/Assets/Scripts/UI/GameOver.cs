using System;
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

    public bool Check(float userMass)
    {
        if (userMass < 0)
        {
            gameOver = true;
            gameObject.SetActive(true);
            var textComponent = gameObject.GetComponentInChildren<TMP_Text>();
            textComponent.text = $"УР {GlobalVariables.Instance.currentLevel.ToString()}";

            var audioSource = gameObject.GetComponent<AudioSource>();
            audioSource.time = 0.1f;
            audioSource.Play();

            Time.timeScale = 0f;
            CoroutineManager.Instance.StopAllManagedCoroutines();
            HistoryManager.Instance.Clear();
            positiveSpawner.CancelInvokes();
            subtractorSpawner.CancelInvokes();
            GlobalVariables.Instance.speedScale = GlobalVariables.speedDefault;
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
        playerManager.MoveToDefaultPosition();
        gameOver = false;
        gameObject.SetActive(false);
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}