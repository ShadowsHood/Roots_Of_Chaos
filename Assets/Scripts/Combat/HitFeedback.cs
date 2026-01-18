using UnityEngine;
using System.Collections;

public class HitFeedback : MonoBehaviour
{
    private SpriteRenderer sr;
    private Vector3 originalScale;
    public Color hitColor = Color.red;
    public float flashDuration = 0.1f;
    public float scaleMultiplier = 1.2f;
    private Coroutine currentRoutine;

    void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
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
}
