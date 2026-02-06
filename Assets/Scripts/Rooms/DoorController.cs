using UnityEngine;

public enum DoorDirection
{
    North,
    South,
    East,
    West
}

public class DoorController : MonoBehaviour
{
    public DoorDirection direction;
    public GameObject doorCollider;
    private bool locked = false;
    // private bool hidden = false;
    public Sprite doorOpen;
    public Sprite doorClosed;
    private SpriteRenderer spriteRenderer;

    void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer != null && doorOpen != null)
        {
            spriteRenderer.sprite = doorOpen;
        }
    }
    private void Start()
    {
        UpdateDoorCollider();
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && !locked)
        {
            RoomManager.Instance.EnterDoor(direction);
        }
    }

    public void Lock()
    {
        locked = true;
        UpdateDoorCollider();
    }
    public void Lock(float duration)
    {
        locked = true;
        UpdateDoorCollider();
        Invoke(nameof(Unlock), duration);
    }

    public void Unlock()
    {
        locked = false;
        UpdateDoorCollider();
    }

    private void UpdateDoorCollider()
    {
        if (doorCollider != null) doorCollider.SetActive(locked);
        if (spriteRenderer != null)
        {
            if (locked && doorClosed != null)
                spriteRenderer.sprite = doorClosed;
            else if (!locked && doorOpen != null)
                spriteRenderer.sprite = doorOpen;
        }
    }
}
