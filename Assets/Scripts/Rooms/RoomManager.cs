using UnityEngine;
using System.Collections.Generic;

public class RoomManager : MonoBehaviour
{
    public static RoomManager Instance;
    public int currentRoomIndex;
    public Dictionary<int, Room> allRooms = new Dictionary<int, Room>();

    void Awake()
    {
        Instance = this;
    }

    public void AddRoom(Room room)
    {
        allRooms[room.mapIndex] = room;
    }
    public Room GetRoom(int index)
    {
        if (allRooms.TryGetValue(index, out var room))
            return room;
        return null;
    }

    public void EnterDoor(DoorDirection dir)
    {
        int nextIndex = GetNextRoomIndex(currentRoomIndex, dir);
        Room nextRoom = GetRoom(nextIndex);
        if (nextRoom == null)
        {
            Debug.LogWarning("La salle suivante n'existe pas !");
            return;
        }

        MoveToRoom(nextRoom, nextIndex);
    }

    int GetNextRoomIndex(int current, DoorDirection dir)
    {
        switch (dir)
        {
            case DoorDirection.North: return current - 10;
            case DoorDirection.South: return current + 10;
            case DoorDirection.East: return current + 1;
            case DoorDirection.West: return current - 1;
        }
        return current;
    }

    void MoveToRoom(Room r, int index)
    {
        currentRoomIndex = index;

        // Camera
        Camera.main.transform.position = new Vector3(
            r.cameraFocusPoint.position.x,
            r.cameraFocusPoint.position.y,
            -10f
        );

        // Player
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        player.transform.position = r.playerSpawnPoint.position;

        Debug.Log("Entrée dans la salle: " + index);
    }
}
