using UnityEngine;

public class RevealPlatform : MonoBehaviour
{
    public SpriteRenderer platformRenderer;
    public Collider2D solidCollider;

    private Collider2D revealTrigger;
    private DashedOutline2D dashedOutline;
    private bool revealed;

    void Awake()
    {
        revealTrigger = GetComponent<Collider2D>();
        dashedOutline = transform.parent != null
            ? transform.parent.GetComponentInChildren<DashedOutline2D>()
            : null;
        platformRenderer.enabled = false;
        solidCollider.enabled = false;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (revealed || !other.CompareTag("Player"))
            return;

        revealed = true;
        platformRenderer.enabled = true;
        solidCollider.enabled = true;
        revealTrigger.enabled = false;
        if (dashedOutline != null) dashedOutline.SetVisible(false);
    }
}
