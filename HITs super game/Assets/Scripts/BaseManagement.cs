using System.Collections;
using System.Collections.Generic;
using UnityEditor.Build;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Tilemaps;

public class Room
{
    Vector2Int leftDownCorner;
    Vector2Int rightUpCorner;
    Vector2Int center;

    private bool isWithNPC = false;

    public Room(Vector2Int leftDownCorner, Vector2Int rightUpCorner)
    {
        this.leftDownCorner = leftDownCorner;
        this.rightUpCorner = rightUpCorner;

        center = new Vector2Int((leftDownCorner.x + rightUpCorner.x) / 2, (leftDownCorner.y + rightUpCorner.y) / 2);
    }

    public Vector2Int GetLeftDownCorner()
    {
        return leftDownCorner;
    }

    public Vector2Int GetRightUpCorner()
    {
        return rightUpCorner;
    }

    public Vector2Int GetCenter()
    {
        return center;
    }

    public void SetNpc()
    {
        isWithNPC = true;
    }

    public bool CheckNPC()
    {
        return isWithNPC;
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

    public static int GetRoomsAmount()
    {
        return rooms.Count;
    }

    public static Room getRoomByID(int id)
    {
        return rooms[id];
    }

    public static void SetNpc(int id, GameObject NPC)
    {
        rooms[id].SetNpc();
        NPC.transform.position = new Vector3(rooms[id].GetCenter().x, rooms[id].GetCenter().y, NPC.transform.position.z);
        NPC.GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.None;
        NPC.GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.FreezeRotation;
        NPC.GetComponent<BoxCollider2D>().isTrigger = false;
        NPC.transform.Find("cage").gameObject.SetActive(false);
    }
}

public class BaseManagement : MonoBehaviour
{
    [SerializeField] private Tilemap tilemap;
    [SerializeField] private GameObject HomeIndicator;
    private bool homeModOn = false;

    public void ChangeSetHomeMod()
    {
        homeModOn = !homeModOn;
        HomeIndicator.SetActive(homeModOn);
    }

    void Update()
    {
        if (homeModOn)
        {
            HomeIndicator.transform.position = Input.mousePosition;

            if (Input.GetKeyDown(KeyCode.Mouse0))
            {
                Vector3 worldPoint = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                Vector3Int cellPosition = tilemap.WorldToCell(worldPoint);

                Base.AddRoom(cellPosition);

                ChangeSetHomeMod();
            }
        }
    }
}