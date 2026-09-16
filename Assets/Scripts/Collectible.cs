using UnityEngine;

public class Collectible : MonoBehaviour
{
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            if (LevelManager.Instance != null) LevelManager.Instance.AddRing();
            Destroy(gameObject);
        }
    }
}