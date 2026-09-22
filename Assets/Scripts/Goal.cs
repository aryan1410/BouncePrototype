using UnityEngine;
using UnityEngine.SceneManagement;

public class Goal : MonoBehaviour
{
    private SpriteRenderer sr;

    void Start()
    {
        sr = GetComponent<SpriteRenderer>();
        // Look locked (gray) if this level requires defeating the enemy
        if (sr != null && LevelManager.Instance != null
            && LevelManager.Instance.requireEnemyDefeat)
            sr.color = Color.gray;
    }

    // Called by LevelManager when the enemy dies
    public void Unlock()
    {
        if (sr != null) sr.color = Color.green;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player") || LevelManager.Instance == null) return;

        if (LevelManager.Instance.CanExit())
        {
            // Level 3 is the final showcase level: display its local completion UI
            // instead of advancing past the final scene.
            if (SceneManager.GetActiveScene().name == "Level3"
                && Level3Completion.Instance != null)
                Level3Completion.Instance.ShowWin();
            else
                LevelManager.Instance.LoadNextLevel();
        }
        // else: still locked — go stomp the enemy first
    }
}
