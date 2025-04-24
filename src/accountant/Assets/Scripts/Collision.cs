using UnityEngine;

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
            player.SetValue(spawnedObject);

            Destroy(gameObject);
        }
    }
}