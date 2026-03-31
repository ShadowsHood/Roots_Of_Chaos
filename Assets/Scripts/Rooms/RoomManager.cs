using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using System;

public class RoomManager : MonoBehaviour
{
    public static RoomManager Instance;
    public int currentRoomIndex;
    public Dictionary<int, Room> allRooms = new Dictionary<int, Room>();
    public event Action OnEnterRoom;

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
        Room currentRoom = GetRoom(currentRoomIndex);
        currentRoom.Exit();

        RoomType currentRoomType = currentRoom.GetRoomType();
        RoomType nextRoomType = nextRoom.GetRoomType();
        bool isStartNormalSwap = (currentRoomType == RoomType.Start && nextRoomType == RoomType.Normal)
                      || (currentRoomType == RoomType.Normal && nextRoomType == RoomType.Start);
        if (!isStartNormalSwap && currentRoomType != nextRoomType)
        {
            switch (nextRoom.GetRoomType())
            {
                case RoomType.Heal:
                    MusicManager.Instance.PlayMusic("Secret");
                    break;
                case RoomType.Forge:
                    MusicManager.Instance.PlayMusic("Shop");
                    break;
                case RoomType.Light:
                    MusicManager.Instance.PlayMusic("Item");
                    break;
                case RoomType.Boss:
                    MusicManager.Instance.PlayMusic("Boss");
                    break;
                default:
                    MusicManager.Instance.PlayMusic("Floor");
                    break;
            }
        }

        MoveToRoom(nextRoom, nextIndex, dir);
    }

    private int GetNextRoomIndex(int current, DoorDirection dir)
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

    public void MoveToRoom(Room r, int index, DoorDirection? dir = null)
    {
        if (r == null) return;
        currentRoomIndex = index;
        // Camera
        if (CameraController.Instance != null)
        {
            CameraController.Instance.target = r.cameraFocusPoint;
        }

        // Player
        if (PlayerController.Instance != null)
        {
            Transform spawnPoint = null;
            if (dir.HasValue)
                spawnPoint = r.GetSpawnPointFrom(dir.Value);

            if (spawnPoint != null)
                PlayerController.Instance.transform.position = spawnPoint.position;
            else if (r.center != null)
                PlayerController.Instance.transform.position = r.center.position;

        }
        r.Enter();
        OnEnterRoom?.Invoke();
        MinimapController.Instance.UpdateMinimap();
        Debug.Log("Entering room: " + index);
    }
}
