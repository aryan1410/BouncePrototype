using UnityEngine;

public class Enemy : MonoBehaviour
{
    [Header("Patrol")]
    public float moveSpeed = 2f;
    public float edgeCheckDistance = 0.6f;  // how far below the front foot to look for ground
    public float wallCheckDistance = 0.5f;  // how far ahead to look for walls/spikes
    public LayerMask groundLayer;           // set to "Ground"
    public LayerMask hazardLayer;           // set to "Hazard"

    [Header("Stomp")]
    public float ballBounceForce = 12f;     // how hard the ball bounces after a stomp

    private Rigidbody2D rb;
    private int dir = 1; // 1 = moving right, -1 = moving left

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void FixedUpdate()
    {
        // Patrol horizontally
        rb.linearVelocity = new Vector2(dir * moveSpeed, 0f);

        // Is there still ground ahead of my feet? (stops me falling off edges)
        Vector2 frontFoot = (Vector2)transform.position
                            + Vector2.right * dir * wallCheckDistance
                            + Vector2.down * 0.5f;
        bool groundAhead = Physics2D.Raycast(frontFoot, Vector2.down, edgeCheckDistance, groundLayer);

        // Is there a wall or spikes right in front of me?
        Vector2 eye = transform.position;
        bool wallAhead = Physics2D.Raycast(eye, Vector2.right * dir, wallCheckDistance, groundLayer);
        bool spikeAhead = Physics2D.Raycast(eye, Vector2.right * dir, wallCheckDistance, hazardLayer);

        // Turn around if I'd fall, hit a wall, or hit spikes
        if (!groundAhead || wallAhead || spikeAhead)
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
        Vector2 frontFoot = (Vector2)transform.position
                            + Vector2.right * dir * wallCheckDistance
                            + Vector2.down * 0.5f;
        Gizmos.DrawLine(frontFoot, frontFoot + Vector2.down * edgeCheckDistance);
        Gizmos.DrawLine(transform.position,
                        (Vector2)transform.position + Vector2.right * dir * wallCheckDistance);
    }
}
