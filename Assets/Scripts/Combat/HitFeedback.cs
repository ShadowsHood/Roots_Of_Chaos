using UnityEngine;
using System.Collections;

public class HitFeedback : MonoBehaviour
{
    public Color hitColor = Color.red;
    public float flashDuration = 0.1f;
    public float scaleMultiplier = 1.2f;
    public float knockbackForce = 3f;
    private SpriteRenderer sr;
    private Rigidbody2D rb;
    private Vector3 originalScale;
    private Coroutine currentRoutine;

    void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
        rb = GetComponent<Rigidbody2D>();
        originalScale = transform.localScale;
    }

    public void PlayHitEffect()
    {
        if (currentRoutine != null) StopCoroutine(currentRoutine);
        currentRoutine = StartCoroutine(HitCoroutine());
    }

    private IEnumerator HitCoroutine()
    {
        // Change color
        sr.color = hitColor;

        // Scale up
        transform.localScale = originalScale * scaleMultiplier;

        yield return new WaitForSeconds(flashDuration);

        // Revert
        sr.color = Color.white;
        transform.localScale = originalScale;
        currentRoutine = null;
    }

    public void Knockback(Vector2 direction)
    {
        rb.AddForce(direction * knockbackForce, ForceMode2D.Impulse);
    }
}
