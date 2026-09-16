using UnityEngine;

public class BallController : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed = 6f;
    public float jumpForce = 12f;

    [Header("Ground Check")]
    public float groundCheckDistance = 0.55f; // roughly the ball's radius
    public float groundCheckRadius = 0.2f;
    public LayerMask groundLayer;             // set this to your "Ground" layer

    private Rigidbody2D rb;
    private bool isGrounded;
    private bool flipped = false;
    private float baseGravity;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        baseGravity = Mathf.Abs(rb.gravityScale);
        if (baseGravity == 0f) baseGravity = 3f;
    }

    void Update()
    {
        // Which way is "down" right now? (flips with gravity)
        Vector2 downDir = flipped ? Vector2.up : Vector2.down;
        Vector2 checkPos = (Vector2)transform.position + downDir * groundCheckDistance;
        isGrounded = Physics2D.OverlapCircle(checkPos, groundCheckRadius, groundLayer);

        // Jump (Space) — jumps "up" relative to current gravity
        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            float jumpDir = flipped ? -1f : 1f;
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce * jumpDir);
        }

        // THE TWIST: flip gravity (Left Shift)
        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            flipped = !flipped;
            rb.gravityScale = baseGravity * (flipped ? -1f : 1f);
        }
    }

    void FixedUpdate()
    {
        float x = Input.GetAxisRaw("Horizontal"); // A/D or arrow keys
        rb.linearVelocity = new Vector2(x * moveSpeed, rb.linearVelocity.y);
    }

    // Called by LevelManager when the ball respawns
    public void ResetGravity()
    {
        flipped = false;
        rb.gravityScale = baseGravity;
    }

    // Yellow circle in the editor shows where "ground" is being checked
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Vector2 downDir = (Application.isPlaying && flipped) ? Vector2.up : Vector2.down;
        Vector2 checkPos = (Vector2)transform.position + downDir * groundCheckDistance;
        Gizmos.DrawWireSphere(checkPos, groundCheckRadius);
    }
}