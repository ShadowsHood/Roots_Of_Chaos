using UnityEngine;

public class Room : MonoBehaviour
{
    public int mapIndex;
    public Transform cameraFocusPoint;
    public Transform playerSpawnPoint;

    [Header("Doors")]
    public GameObject doorNorth;
    public GameObject doorSouth;
    public GameObject doorEast;
    public GameObject doorWest;

    public void UpdateDoors()
    {
        FloorMap map = FloorMap.Instance;
        int width = 10;
        bool north = HasRoom(mapIndex - width);
        bool south = HasRoom(mapIndex + width);
        bool east = (mapIndex % width != width - 1) && HasRoom(mapIndex + 1);
        bool west = (mapIndex % width != 0) && HasRoom(mapIndex - 1);

        SetDoorActive(doorNorth, north);
        SetDoorActive(doorSouth, south);
        SetDoorActive(doorEast, east);
        SetDoorActive(doorWest, west);

        // if (!map.rooms[mapIndex].IsCleared())
        //     CloseAllDoors();
        // else
        //     OpenAllDoors();
    }

    private bool HasRoom(int index)
    {
        if (index < 0 || index >= FloorMap.Instance.rooms.Length)
            return false;

        return FloorMap.Instance.rooms[index].filled;
    }

    private void SetDoorActive(GameObject door, bool state)
    {
        if (door != null)
            door.SetActive(state);
    }

    private void CloseAllDoors()
    {
        if (doorNorth) doorNorth.SetActive(false);
        if (doorSouth) doorSouth.SetActive(false);
        if (doorEast) doorEast.SetActive(false);
        if (doorWest) doorWest.SetActive(false);
    }

    private void OpenAllDoors()
    {
        if (doorNorth) doorNorth.SetActive(true);
        if (doorSouth) doorSouth.SetActive(true);
        if (doorEast) doorEast.SetActive(true);
        if (doorWest) doorWest.SetActive(true);
    }
}
