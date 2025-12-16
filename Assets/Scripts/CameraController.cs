using UnityEngine;

public class CameraController : MonoBehaviour
{
    public static CameraController Instance;
    public Transform target;
    Vector3 velocity = Vector3.zero;
    public float speed = 2f;

    void Awake()
    {
        Instance = this;
    }
    void Update()
    {
        if (target != null)
        {
            Vector3 targetPos = new Vector3(target.position.x, target.position.y, -10f);
            transform.position = Vector3.SmoothDamp(transform.position, targetPos, ref velocity, speed);
        }
    }
}
