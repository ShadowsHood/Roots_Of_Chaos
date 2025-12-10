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
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.CompareTag("Player")) return;
        RoomManager.Instance.EnterDoor(direction);
    }
}
