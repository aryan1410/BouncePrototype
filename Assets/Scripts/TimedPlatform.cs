using System.Collections;
using UnityEngine;

public class TimedPlatform : MonoBehaviour
{
    [Min(0f)]
    public float lifetime = 5f;

    [Min(0f)]
    public float blinkDuration = 3f;

    [Min(0.01f)]
    public float blinkInterval = 0.15f;

    private SpriteRenderer platformRenderer;
    private Collider2D platformCollider;
    private bool countdownStarted;

    void Awake()
    {
        platformRenderer = GetComponent<SpriteRenderer>();
        platformCollider = GetComponent<Collider2D>();
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (countdownStarted || !collision.collider.CompareTag("Player"))
            return;

        countdownStarted = true;
        StartCoroutine(DisableAfterLifetime());
    }

    private IEnumerator DisableAfterLifetime()
    {
        float normalDuration = Mathf.Max(0f, lifetime - blinkDuration);
        yield return new WaitForSeconds(normalDuration);

        float elapsed = 0f;
        while (elapsed < blinkDuration)
        {
            platformRenderer.enabled = !platformRenderer.enabled;
            yield return new WaitForSeconds(blinkInterval);
            elapsed += blinkInterval;
        }

        platformRenderer.enabled = false;
        platformCollider.enabled = false;
    }
}
