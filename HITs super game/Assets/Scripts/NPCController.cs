using System.Collections;
using System.Collections.Generic;
using UnityEditor.U2D.Aseprite;
using UnityEngine;
using UnityEngine.UI;

public class NPCController : MonoBehaviour
{
    private Camera cam;
    private GameObject ChoosingRoomUI;

    private bool isChoosingRoom = false;
    private int currRoom = 0;

    // Start is called before the first frame update
    void Start()
    {
        cam = Camera.main;
        ChoosingRoomUI = GameObject.Find("ChooseRoomObj").transform.Find("ChooseRoomPanel").gameObject;
        ChoosingRoomUI.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Escape) && isChoosingRoom)
        {
            StopChoosingRoom();
        }
    }

    public void ChooseRoom()
    {
        if (Base.GetRoomsAmount() == 0)
            return;

        isChoosingRoom = true;

        ChoosingRoomUI.SetActive(true);
        ChoosingRoomUI.transform.Find("Left").GetComponentInChildren<Button>().onClick.RemoveAllListeners();
        ChoosingRoomUI.transform.Find("Left").GetComponentInChildren<Button>().onClick.AddListener(LeftRoom);

        ChoosingRoomUI.transform.Find("Right").GetComponentInChildren<Button>().onClick.RemoveAllListeners();
        ChoosingRoomUI.transform.Find("Right").GetComponentInChildren<Button>().onClick.AddListener(RightRoom);

        ChoosingRoomUI.transform.Find("Stop").GetComponentInChildren<Button>().onClick.RemoveAllListeners();
        ChoosingRoomUI.transform.Find("Stop").GetComponentInChildren<Button>().onClick.AddListener(StopChoosingRoom);

        ChoosingRoomUI.transform.Find("Confirm").GetComponentInChildren<Button>().onClick.RemoveAllListeners();
        ChoosingRoomUI.transform.Find("Confirm").GetComponentInChildren<Button>().onClick.AddListener(ConfirmRoom);
        Time.timeScale = 0;

        currRoom = 0;
        ShowRoom();
    }

    public void StopChoosingRoom()
    {
        GameObject.FindGameObjectWithTag("World").GetComponent<WorldGeneration>().ClearMapByPosition(cam.transform.position);

        isChoosingRoom = false;

        ChoosingRoomUI.SetActive(false);
        Time.timeScale = 1;
    }

    public void ConfirmRoom()
    {
        if (!Base.getRoomByID(currRoom).CheckNPC())
        {
            Base.SetNpc(currRoom, this.gameObject);
            StopChoosingRoom();
        }
    }

    void ShowRoom()
    {
        GameObject.FindGameObjectWithTag("World").GetComponent<WorldGeneration>().ClearMapByPosition(cam.transform.position);
        Vector2Int newPos = Base.getRoomByID(currRoom).GetCenter();
        cam.transform.position = new Vector3(newPos.x, newPos.y, cam.transform.position.z);
        GameObject.FindGameObjectWithTag("World").GetComponent<WorldGeneration>().RenderMapByPosition(cam.transform.position);
    }

    public void RightRoom()
    {
        currRoom = (currRoom + 1 >= Base.GetRoomsAmount()) ? 0 : (currRoom + 1);
        ShowRoom();
    }

    public void LeftRoom()
    {
        currRoom = (currRoom - 1 < 0) ? Base.GetRoomsAmount() - 1 : (currRoom - 1);
        ShowRoom();
    }

    void OnMouseOver()
    {
        if (Input.GetKeyDown(KeyCode.Mouse0)){
            ChooseRoom();
        }
    }
}
