using SaveData;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Tilemaps;

[System.Serializable]
public class Room
{
    [SerializeField] public Vector2Int leftDownCorner;
    [SerializeField] public Vector2Int rightUpCorner;
    [SerializeField] public Vector2Int center;

    [SerializeField] public bool isWithNPC = false;
    [SerializeField] private GameObject NPC = null;

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

    public void SetNpc(GameObject NPC)
    {
        isWithNPC = true;
        this.NPC = NPC;
        this.NPC.GetComponent<NPCController>().isInCage = false;
    }

    public void SetEmptyNpc()
    {
        isWithNPC = true;
    }

    public bool CheckNPC()
    {
        return isWithNPC;
    }

    public void DeleteNPC()
    {
        if(NPC != null)
        {
            NPC.GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.FreezePositionY | RigidbodyConstraints2D.FreezePositionX | RigidbodyConstraints2D.FreezeRotation;
            NPC.GetComponent<BoxCollider2D>().isTrigger = true;
            NPC.transform.Find("cage").gameObject.SetActive(true);
            NPC.GetComponent<NPCController>().isInCage = true;
        }
    }
}

public static class Base
{
    private static List<Room> rooms = new List<Room>();

    public static List<Room> GetRooms()
    {
        return rooms;
    }

    public static void SetRooms(List<Room> newRooms)
    {
        rooms = newRooms;
    }

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
        room.DeleteNPC();
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
        rooms[id].SetNpc(NPC);
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
    [SerializeField] private GameObject NPCPrefab;
    private bool homeModOn = false;
    private string saveKey = "mainSaveBase";

    private SaveData.Base GetSaveSnapshot()
    {
        var data = new SaveData.Base()
        {
            rooms = Base.GetRooms(),
        };

        return data;
    }

    public void Save()
    {
        SaveManager.Save(saveKey, GetSaveSnapshot());
    }

    public void Load()
    {
        var data = SaveManager.Load<SaveData.Base>(saveKey);
        Base.SetRooms(data.rooms);

        for(int i = 0; i < Base.GetRooms().Count; i++)
        {
            if (Base.getRoomByID(i).CheckNPC())
            {
                GameObject npc = Instantiate(NPCPrefab);
                Base.SetNpc(i, npc);
            }
        }
    }

    public void ChangeSetHomeMod()
    {
        homeModOn = !homeModOn;
        HomeIndicator.SetActive(homeModOn);
    }

    private void Start()
    {
        Load();
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