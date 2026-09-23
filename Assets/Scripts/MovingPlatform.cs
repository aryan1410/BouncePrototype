using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class MovingPlatform : MonoBehaviour
{
    [Header("Movement")]
    [Tooltip("Distance to travel from the placed start position before returning.")]
    public Vector2 moveOffset = new Vector2(5f, 0f);

    [Tooltip("Travel speed in world units per second.")]
    public float moveSpeed = 2f;

    private Rigidbody2D rb;
    private Vector2 startPosition;
    private float elapsed;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        startPosition = rb.position;
    }

    void FixedUpdate()
    {
        float travelDistance = moveOffset.magnitude;
        if (travelDistance <= Mathf.Epsilon || moveSpeed <= 0f)
            return;

        elapsed += Time.fixedDeltaTime;
        float progress = Mathf.PingPong(elapsed * moveSpeed / travelDistance, 1f);
        rb.MovePosition(Vector2.Lerp(startPosition, startPosition + moveOffset, progress));
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(transform.position, (Vector2)transform.position + moveOffset);
    }
}
