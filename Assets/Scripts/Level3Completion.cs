using UnityEngine;
using UnityEngine.SceneManagement;

public class Level3Completion : MonoBehaviour
{
    public static Level3Completion Instance;

    [SerializeField] private GameObject winPopup;
    [SerializeField] private GameObject player;

    void Awake()
    {
        Instance = this;
        if (winPopup != null) winPopup.SetActive(false);
    }

    public void ShowWin()
    {
        if (winPopup == null || winPopup.activeSelf) return;

        Rigidbody2D rb = player != null ? player.GetComponent<Rigidbody2D>() : null;
        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero;
            rb.simulated = false;
        }

        BallController controller = player != null ? player.GetComponent<BallController>() : null;
        if (controller != null) controller.enabled = false;

        winPopup.SetActive(true);
    }

    public void PlayAgain()
    {
        SceneManager.LoadScene("Level3");
    }
}
