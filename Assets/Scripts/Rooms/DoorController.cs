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
        if (collision.CompareTag("Player") && !locked)
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

    public void Unlock()
    {
        locked = false;
    }
}
