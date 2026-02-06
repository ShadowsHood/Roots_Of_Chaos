using UnityEngine;
using System.Collections;

public class HitFeedback : MonoBehaviour
{
    public Color hitColor = Color.red;
    public float flashDuration = 0.1f;
    public float scaleMultiplier = 1.2f;
    public float knockbackForce = 3f;
    private SpriteRenderer sr;
    private SpriteRenderer[] spriteRenderers;
    private Rigidbody2D rb;
    private Vector3 originalScale;
    private Coroutine currentRoutine;

    void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
        if (sr == null)
        {
            spriteRenderers = GetComponentsInChildren<SpriteRenderer>();
        }
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
        if (sr != null)
            sr.color = hitColor;
        else if (spriteRenderers != null)
        {
            foreach (var s in spriteRenderers)
                s.color = hitColor;
        }

        // Scale up
        transform.localScale = originalScale * scaleMultiplier;

        yield return new WaitForSeconds(flashDuration);

        // Revert
        if (sr != null)
            sr.color = Color.white;
        else if (spriteRenderers != null)
        {
            foreach (var s in spriteRenderers)
                s.color = Color.white;
        }
        transform.localScale = originalScale;
        currentRoutine = null;
    }

    public void Knockback(Vector2 direction)
    {
        rb.AddForce(direction * knockbackForce, ForceMode2D.Impulse);
    }
}
