using UnityEngine;

public class OrientationManager : MonoBehaviour
{
    public GameObject orientationPanel;

    // StartCoroutine тут не подходит, т.к. теряется при orientationPanel.SetActive(false)
    void Update()
    {
        bool isPortrait = Screen.width <= Screen.height;
        if (isPortrait)
        {
            orientationPanel.SetActive(false);
            Time.timeScale = 1f;
        }
        else
        {
            orientationPanel.SetActive(true);
            Time.timeScale = 0f; // Остановить игру при неправильной ориентации
        }
    }
}