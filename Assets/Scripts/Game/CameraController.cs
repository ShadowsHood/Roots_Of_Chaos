using UnityEngine;
using System.Collections;

public class CameraController : MonoBehaviour
{
    public static CameraController Instance;
    public Transform target;
    Vector3 velocity = Vector3.zero;
    public float speed = 2f;

    private float roomWidth = 15f * 1.3f;
    private float roomHeight = 9f * 1.3f;

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        float targetAspect = roomWidth / roomHeight;
        float windowAspect = (float)Screen.width / Screen.height;
        float scaleHeight = windowAspect / targetAspect;

        Camera cam = Camera.main;
        if (scaleHeight < 1f)
        {
            cam.orthographicSize = roomHeight / 2f / scaleHeight;
        }
        else
        {
            cam.orthographicSize = roomHeight / 2f;
        }
    }

    void Update()
    {
        if (target != null)
        {
            Vector3 targetPos = new Vector3(target.position.x, target.position.y, -10f);
            transform.position = Vector3.SmoothDamp(transform.position, targetPos, ref velocity, speed);
        }
    }

    public void Shake(float duration, float magnitude)
    {
        StartCoroutine(ShakeCoroutine(duration, magnitude));
    }
    IEnumerator ShakeCoroutine(float duration, float magnitude)
    {
        Vector3 originalPos = transform.position;
        // Vector3 impactDir = Random.insideUnitCircle.normalized;
        float impactDir = Random.value < 0.5f ? -1f : 1f;
        Vector3 impactOffset = new Vector3(impactDir * magnitude, 0f, 0f);
        float elapsed = 0f;

        while (elapsed < duration)
        {
            float t = elapsed / duration;
            float ease = 1f - Mathf.Pow(t, 3);

            transform.position = Vector3.Lerp(
                originalPos,
                originalPos + impactOffset,
                ease
            );

            elapsed += Time.deltaTime;
            yield return null;
        }

        transform.position = originalPos;
    }
}
