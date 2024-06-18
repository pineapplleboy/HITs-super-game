using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Tilemaps;

public class Room
{
    Vector2Int leftDownCorner;
    Vector2Int rightUpCorner;

    public Room(Vector2Int leftDownCorner, Vector2Int rightUpCorner)
    {
        this.leftDownCorner = leftDownCorner;
        this.rightUpCorner = rightUpCorner;
    }

    public Vector2Int GetLeftDownCorner()
    {
        return leftDownCorner;
    }

    public Vector2Int GetRightUpCorner()
    {
        return rightUpCorner;
    }
}

public static class Base
{
    private static List<Room> rooms = new List<Room>();

    public static void AddRoom(Vector3Int cellPosition)
    {
        int x = cellPosition.x;
        int y = cellPosition.y;

        if (GetRoomFromCoords(x, y) != null)
            return;

        Vector2Int[] newRoom = GameObject.FindGameObjectWithTag("World").GetComponent<WorldGeneration>().CheckPlaceForRoom(cellPosition);
        if(newRoom != null)
        {
            rooms.Add(new Room(newRoom[0], newRoom[1]));
            GameObject.FindGameObjectWithTag("World").GetComponent<WorldGeneration>().SetRoom(rooms[rooms.Count - 1]);
        }
    }

    public static Room GetRoomFromCoords(int x, int y)
    {
        foreach (Room room in rooms)
        {
            if (x >= room.GetLeftDownCorner().x && x <= room.GetRightUpCorner().x &&
                y >= room.GetLeftDownCorner().y && y <= room.GetRightUpCorner().y)
            {
                return room;
            }
        }

        return null;
    }

    public static void DestroyRoom(Room room)
    {
        rooms.Remove(room);
    }
}

public class BaseManagement : MonoBehaviour
{
    [SerializeField] private Tilemap tilemap;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            Vector3 worldPoint = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector3Int cellPosition = tilemap.WorldToCell(worldPoint);

            Base.AddRoom(cellPosition);
        }
    }
}