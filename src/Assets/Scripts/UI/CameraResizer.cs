using UnityEngine;

public class CameraResizer : MonoBehaviour
{
    public float targetWidth = 8f; // ширина игрового поля в Unity-единицах

    void Start()
    {
        Camera cam = Camera.main;
        float screenRatio = (float)Screen.width / (float)Screen.height;
        float targetRatio = targetWidth / cam.orthographicSize / 2f;
        // Новое значение orthographicSize для сохранения targetWidth на экране
        cam.orthographicSize = targetWidth / screenRatio / 2f;
    }
}