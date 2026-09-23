using UnityEngine;

public class Enemy : MonoBehaviour
{
    [Header("Patrol")]
    public float moveSpeed = 2f;
    public float edgeCheckDistance = 0.6f;  // how far from the front edge to look for a surface
    public float wallCheckDistance = 0.5f;  // how far ahead to look for walls/spikes
    public LayerMask groundLayer;           // set to "Ground"
    public LayerMask hazardLayer;           // set to "Hazard"
    public bool isCeilingWalker = false;

    [Header("Stomp")]
    public float ballBounceForce = 12f;     // how hard the ball bounces after a stomp

    private Rigidbody2D rb;
    private Collider2D enemyCollider;
    private int dir = 1; // 1 = moving right, -1 = moving left

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        enemyCollider = GetComponent<Collider2D>();
    }

    void FixedUpdate()
    {
        // Patrol horizontally
        rb.linearVelocity = new Vector2(dir * moveSpeed, 0f);

        // Look down for floor walkers and up for ceiling walkers.
        Vector2 surfaceDirection = isCeilingWalker ? Vector2.up : Vector2.down;
        float surfaceOffset = GetSurfaceOffset();
        Vector2 frontFoot = (Vector2)transform.position
                            + Vector2.right * dir * wallCheckDistance
                            + surfaceDirection * surfaceOffset;
        bool surfaceAhead = Physics2D.Raycast(frontFoot, surfaceDirection, edgeCheckDistance, groundLayer);

        // Is there a wall or spikes right in front of me?
        Vector2 eye = transform.position;
        bool wallAhead = Physics2D.Raycast(eye, Vector2.right * dir, wallCheckDistance, groundLayer);
        bool spikeAhead = Physics2D.Raycast(eye, Vector2.right * dir, wallCheckDistance, hazardLayer);

        // Turn around if I'd fall, hit a wall, or hit spikes
        if (!surfaceAhead || wallAhead || spikeAhead)
            dir = -dir;
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (!collision.collider.CompareTag("Player")) return;

        Rigidbody2D ballRb = collision.rigidbody;
        float gSign = Mathf.Sign(ballRb.gravityScale); // +1 normal, -1 when gravity is flipped

        // Previous position/velocity check, kept for reference:
        /*
        // Convert "above" and "falling" into the CURRENT gravity frame:
        float verticalOffset = (collision.transform.position.y - transform.position.y) * gSign;
        float verticalVel = ballRb.linearVelocity.y * gSign;

        // Stomp = ball is on the "up" side of the enemy AND moving "down" onto it
        bool stomped = verticalOffset > 0.15f && verticalVel < 0f;
        */

        // Collision resolution may already have stopped the ball's downward motion.
        // Use the contact direction instead; gSign also supports reversed gravity.
        bool stomped = false;
        for (int i = 0; i < collision.contactCount; i++)
        {
            // In the enemy's callback, a hit on its top produces a downward normal.
            if (collision.GetContact(i).normal.y * gSign < -0.7f)
            {
                stomped = true;
                break;
            }
        }

        if (stomped)
        {
            // Bounce the ball back up (against current gravity) and kill the enemy
            ballRb.linearVelocity = new Vector2(ballRb.linearVelocity.x, ballBounceForce * gSign);
            if (LevelManager.Instance != null) LevelManager.Instance.OnEnemyDefeated();
            Destroy(gameObject);
        }
        else
        {
            // Touched from the side -> the ball bursts
            if (LevelManager.Instance != null) LevelManager.Instance.RespawnPlayer();
        }
    }

    // Shows the patrol rays in the editor so you can tune the distances
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Vector2 surfaceDirection = isCeilingWalker ? Vector2.up : Vector2.down;
        float surfaceOffset = GetSurfaceOffset();
        Vector2 frontFoot = (Vector2)transform.position
                            + Vector2.right * dir * wallCheckDistance
                            + surfaceDirection * surfaceOffset;
        Gizmos.DrawLine(frontFoot, frontFoot + surfaceDirection * edgeCheckDistance);
        Gizmos.DrawLine(transform.position,
                        (Vector2)transform.position + Vector2.right * dir * wallCheckDistance);
    }

    private float GetSurfaceOffset()
    {
        Collider2D collider = enemyCollider != null ? enemyCollider : GetComponent<Collider2D>();
        return collider != null ? Mathf.Max(0f, collider.bounds.extents.y - 0.02f) : 0.5f;
    }
}
