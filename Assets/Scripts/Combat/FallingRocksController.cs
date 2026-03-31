using UnityEngine;
using System.Collections;

public class FallingRocksController : MonoBehaviour
{
    private ProjectileData rockData;
    private Transform visualChild;
    private SpriteRenderer visualSpriteRenderer; // ← Ajouté pour le fade
    private CircleCollider2D damageCollider;

    [Header("Fall Physics")]
    private float height = 10f;
    private float verticalVelocity = 0f;
    private float gravity = -20f;
    
    [Header("Fade In")]
    public float fadeInDuration = 0.5f; // Durée du fade in

    [Header("Warning")]
    public GameObject warningIndicatorPrefab;
    private GameObject warningInstance;
    public float warningDuration = 1f;

    [Header("Impact")]
    public float impactRadius = 1.5f;
    public GameObject impactEffectPrefab;

    private bool isFalling = false;
    private bool hasImpacted = false;

    public void Initialize(ProjectileData data, Vector2 targetPosition)
    {
        rockData = data;

        transform.position = new Vector3(targetPosition.x, targetPosition.y, 0);

        // Setup du visual
        if (transform.childCount > 0)
        {
            visualChild = transform.GetChild(0);
            visualSpriteRenderer = visualChild.GetComponent<SpriteRenderer>();
            
            // Commence invisible
            if (visualSpriteRenderer != null)
            {
                Color c = visualSpriteRenderer.color;
                c.a = 0f;
                visualSpriteRenderer.color = c;
            }
        }

        // Setup du collider
        damageCollider = GetComponent<CircleCollider2D>();
        if (damageCollider != null)
        {
            damageCollider.enabled = false;
            damageCollider.radius = impactRadius;
            damageCollider.isTrigger = true;
        }

        // Affiche le warning puis commence la chute
        StartCoroutine(WarningRoutine());
    }

    private IEnumerator WarningRoutine()
    {
        // Crée l'indicateur de warning
        if (warningIndicatorPrefab != null)
        {
            warningInstance = Instantiate(warningIndicatorPrefab, transform.position, Quaternion.identity);

            // Animation de pulsation du warning
            float timer = 0f;
            while (timer < warningDuration)
            {
                timer += Time.deltaTime;
                float scale = 1f + Mathf.Sin(timer * 10f) * 0.2f;
                warningInstance.transform.localScale = Vector3.one * scale * impactRadius;
                yield return null;
            }

            Destroy(warningInstance);
        }
        else
        {
            yield return new WaitForSeconds(warningDuration);
        }

        // Commence le fade in + chute
        StartCoroutine(FadeInRoutine());
        isFalling = true;
    }

    private IEnumerator FadeInRoutine()
    {
        if (visualSpriteRenderer == null) yield break;
        
        float elapsed = 0f;
        Color c = visualSpriteRenderer.color;
        
        while (elapsed < fadeInDuration)
        {
            elapsed += Time.deltaTime;
            c.a = Mathf.Lerp(0f, 1f, elapsed / fadeInDuration);
            visualSpriteRenderer.color = c;
            yield return null;
        }
        
        // Assure que c'est complètement opaque à la fin
        c.a = 1f;
        visualSpriteRenderer.color = c;
    }

    void Update()
    {
        if (!isFalling || hasImpacted) return;

        // Physique de chute
        verticalVelocity += gravity * Time.deltaTime;
        height += verticalVelocity * Time.deltaTime;

        // Update position visuelle
        if (visualChild != null)
        {
            Vector3 pos = transform.position;
            visualChild.position = new Vector3(pos.x, pos.y + height, pos.z);

            // Rotation pendant la chute
            visualChild.Rotate(0, 0, 200f * Time.deltaTime);
        }

        // Impact au sol
        if (height <= 0f)
        {
            Impact();
        }
    }

    private void Impact()
    {
        hasImpacted = true;

        // Active le collider pour infliger des dégâts
        if (damageCollider != null)
            damageCollider.enabled = true;

        // Effet visuel d'impact
        if (impactEffectPrefab != null)
        {
            Instantiate(impactEffectPrefab, transform.position, Quaternion.identity);
        }

        // Le rocher reste au sol un instant puis disparaît
        StartCoroutine(DestroyAfterImpact());
    }

    private IEnumerator DestroyAfterImpact()
    {
        // Désactive le collider après un court instant
        yield return new WaitForSeconds(0.1f);
        if (damageCollider != null)
            damageCollider.enabled = false;

        // Détruit le rocher après un délai
        yield return new WaitForSeconds(0.5f);
        Destroy(gameObject);
    }

    void OnTriggerEnter2D(Collider2D col)
    {
        if (!hasImpacted) return;

        if (col.CompareTag(rockData.targetTag))
        {
            if (PlayerController.Instance != null)
            {
                PlayerController.Instance.TakeDamage(rockData.damage, transform.position);
            }
        }
    }
}