using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.UI;

public class LevelManager : MonoBehaviour
{
    public static LevelManager Instance;

    public GameObject player;      // drag your Player here
    public Transform spawnPoint;   // drag your SpawnPoint here

    [Header("Level Rules")]
    public bool requireEnemyDefeat = true; // exit stays locked until the enemy dies

    private bool enemyDefeated = false;
    private int ringsCollected = 0;
    private int totalRings;
    private TMP_Text coinCounterText;

    void Awake() { Instance = this; }

    void Start()
    {
        if (player != null && spawnPoint != null)
            player.transform.position = spawnPoint.position;

        // FindObjectsByType is the current Unity API; it replaces the deprecated
        // Object.FindObjectsOfType call.
        totalRings = FindObjectsByType<Collectible>(FindObjectsSortMode.None).Length;
        CreateCoinCounter();
        UpdateCoinCounter();
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
        UpdateCoinCounter();
    }

    private void CreateCoinCounter()
    {
        var canvasObject = new GameObject("Coin Counter Canvas", typeof(Canvas), typeof(CanvasScaler));
        Canvas canvas = canvasObject.GetComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 100;

        CanvasScaler scaler = canvasObject.GetComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920, 1080);

        var textObject = new GameObject("Coin Counter", typeof(RectTransform), typeof(TextMeshProUGUI));
        textObject.transform.SetParent(canvasObject.transform, false);

        RectTransform rect = textObject.GetComponent<RectTransform>();
        rect.anchorMin = new Vector2(0f, 1f);
        rect.anchorMax = new Vector2(0f, 1f);
        rect.pivot = new Vector2(0f, 1f);
        rect.anchoredPosition = new Vector2(32f, -32f);
        rect.sizeDelta = new Vector2(500f, 70f);

        coinCounterText = textObject.GetComponent<TextMeshProUGUI>();
        coinCounterText.font = TMP_Settings.defaultFontAsset;
        coinCounterText.fontSize = 42f;
        coinCounterText.alignment = TextAlignmentOptions.TopLeft;
        coinCounterText.color = Color.white;
        coinCounterText.textWrappingMode = TextWrappingModes.NoWrap;
    }

    private void UpdateCoinCounter()
    {
        if (coinCounterText != null)
            coinCounterText.text = $"Coins: {ringsCollected} / {totalRings}";
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
