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
        currentRoomIndex = index;
        // Camera
        CameraController.Instance.target = r.cameraFocusPoint;

        // Player
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        // Collider2D col = player.GetComponent<Collider2D>();
        // col.enabled = false;
        Transform spawnPoint = null;
        if (dir.HasValue)
            spawnPoint = r.GetSpawnPointFrom(dir.Value);
        if (spawnPoint != null)
            player.transform.position = spawnPoint.position;
        else if (r.center != null)
            player.transform.position = r.center.position;

        r.Enter();
        OnEnterRoom?.Invoke();

        // IEnumerator ReenableCollider(Collider2D col, float delay = 0.2f)
        // {
        //     yield return new WaitForSeconds(delay);
        //     col.enabled = true;
        // }
        // StartCoroutine(ReenableCollider(col, 0.5f));
        Debug.Log("Entrée dans la salle: " + index);
    }
}
