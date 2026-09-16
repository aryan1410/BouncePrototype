using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour
{
    public static LevelManager Instance;

    public GameObject player;      // drag your Player here
    public Transform spawnPoint;   // drag your SpawnPoint here

    [Header("Level Rules")]
    public bool requireEnemyDefeat = true; // exit stays locked until the enemy dies
    public Goal goal;                      // (optional) drag the Goal here to recolor it

    private bool enemyDefeated = false;
    private int ringsCollected = 0;

    void Awake() { Instance = this; }

    void Start()
    {
        if (player != null && spawnPoint != null)
            player.transform.position = spawnPoint.position;
    }

    public void RespawnPlayer()
    {
        if (player == null || spawnPoint == null) return;

        Rigidbody2D rb = player.GetComponent<Rigidbody2D>();
        if (rb != null) rb.linearVelocity = Vector2.zero;

        BallController bc = player.GetComponent<BallController>();
        if (bc != null) bc.ResetGravity();

        player.transform.position = spawnPoint.position;
    }

    // Called by Enemy.cs when the ball stomps it
    public void OnEnemyDefeated()
    {
        enemyDefeated = true;
        if (goal != null) goal.Unlock();
    }

    // Goal.cs asks this before letting the player leave
    public bool CanExit()
    {
        return !requireEnemyDefeat || enemyDefeated;
    }

    // Called by Collectible.cs when a ring is picked up
    public void AddRing()
    {
        ringsCollected++;
        Debug.Log("Rings collected: " + ringsCollected);
    }

    public void LoadNextLevel()
    {
        int next = SceneManager.GetActiveScene().buildIndex + 1;
        if (next < SceneManager.sceneCountInBuildSettings)
            SceneManager.LoadScene(next);
        else
            SceneManager.LoadScene(0); // loop back to first scene
    }
}