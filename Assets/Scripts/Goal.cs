using UnityEngine;
using UnityEngine.SceneManagement;

public class Goal : MonoBehaviour
{
    void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player") || LevelManager.Instance == null) return;

        if (LevelManager.Instance.CanExit())
        {
            // Previous scene-name check, kept for reference:
            /*
            // Level 3 is the final showcase level: display its local completion UI
            // instead of advancing past the final scene.
            if (SceneManager.GetActiveScene().name == "Level3"
                && Level3Completion.Instance != null)
                Level3Completion.Instance.ShowWin();
            else
                LevelManager.Instance.LoadNextLevel();
            */

            // The final scene was renamed to Level2; keep showing its completion UI.
            if (SceneManager.GetActiveScene().name == "Level2"
                && Level3Completion.Instance != null)
                Level3Completion.Instance.ShowWin();
            else
                LevelManager.Instance.LoadNextLevel();
        }
        // else: still locked — go stomp the enemy first
    }
}
