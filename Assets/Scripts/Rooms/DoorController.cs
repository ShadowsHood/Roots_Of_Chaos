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
    private bool locked = false;
    // private bool hidden = false;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (locked) return;
        if (collision.CompareTag("Player"))
        {
            RoomManager.Instance.EnterDoor(direction);
        }
    }

    public void Lock()
    {
        locked = true;
    }
    public void Lock(float duration)
    {
        locked = true;
        Invoke(nameof(Unlock), duration);
    }

    void Unlock()
    {
        locked = false;
    }
}
