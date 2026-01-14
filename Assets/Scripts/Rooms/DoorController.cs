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
    }
}
