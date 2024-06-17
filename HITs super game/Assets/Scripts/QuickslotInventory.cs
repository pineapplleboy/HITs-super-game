using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class QuickslotInventory : MonoBehaviour
{
    public Transform quickslotParent;
    public InventoryManager inventoryManager;
    public int currentQuickslotID = 0;
    public Sprite selectedSprite;
    public Sprite notSelectedSprite;

    void Update()
    {
        float mw = Input.GetAxis("Mouse ScrollWheel");
        if (mw > 0.1)
        {
            quickslotParent.GetChild(currentQuickslotID).GetComponent<Image>().sprite = notSelectedSprite;
   
            if (currentQuickslotID >= quickslotParent.childCount - 1)
            {
                currentQuickslotID = 0;
            }
            else
            {
                currentQuickslotID++;
            }
            quickslotParent.GetChild(currentQuickslotID).GetComponent<Image>().sprite = selectedSprite;

        }
        if (mw < -0.1)
        {
            quickslotParent.GetChild(currentQuickslotID).GetComponent<Image>().sprite = notSelectedSprite;
            if (currentQuickslotID <= 0)
            {
                currentQuickslotID = quickslotParent.childCount - 1;
            }
            else
            {
                currentQuickslotID--;
            }
            quickslotParent.GetChild(currentQuickslotID).GetComponent<Image>().sprite = selectedSprite;

        }
        for (int i = 0; i < quickslotParent.childCount; i++)
        {
            if (Input.GetKeyDown((i + 1).ToString()))
            {
                if (currentQuickslotID == i)
                {
                    if (quickslotParent.GetChild(currentQuickslotID).GetComponent<Image>().sprite == notSelectedSprite)
                    {
                        quickslotParent.GetChild(currentQuickslotID).GetComponent<Image>().sprite = selectedSprite;
                    }
                    else
                    {
                        quickslotParent.GetChild(currentQuickslotID).GetComponent<Image>().sprite = notSelectedSprite;
                    }
                }
                else
                {
                    quickslotParent.GetChild(currentQuickslotID).GetComponent<Image>().sprite = notSelectedSprite;
                    currentQuickslotID = i;
                    quickslotParent.GetChild(currentQuickslotID).GetComponent<Image>().sprite = selectedSprite;
                }
            }
        }

        if (Input.GetKeyDown(KeyCode.Mouse1))
        {
            if (quickslotParent.GetChild(currentQuickslotID).GetComponent<InventorySlot>().item != null)
            {
                if (/*quickslotParent.GetChild(currentQuickslotID).GetComponent<InventorySlot>().item.isConsumeable && *//*!inventoryManager.isOpened && */quickslotParent.GetChild(currentQuickslotID).GetComponent<Image>().sprite == selectedSprite)
                {

                    if (quickslotParent.GetChild(currentQuickslotID).GetComponent<InventorySlot>().amount <= 1)
                    {
                        quickslotParent.GetChild(currentQuickslotID).GetComponentInChildren<DragAndDropItem>().NullifySlotData();
                        GameObject.FindGameObjectWithTag("World").GetComponent<WorldGeneration>().SetBlockOnMap(5, quickslotParent.GetChild(currentQuickslotID).GetComponent<InventorySlot>().item);
                    }
                    else
                    {
                        quickslotParent.GetChild(currentQuickslotID).GetComponent<InventorySlot>().amount--;
                        quickslotParent.GetChild(currentQuickslotID).GetComponent<InventorySlot>().itemAmountText.text = quickslotParent.GetChild(currentQuickslotID).GetComponent<InventorySlot>().amount.ToString();
                        GameObject.FindGameObjectWithTag("World").GetComponent<WorldGeneration>().SetBlockOnMap(5, quickslotParent.GetChild(currentQuickslotID).GetComponent<InventorySlot>().item);
                    }
                }
            }
            //else
            //{
            //    GameObject.FindGameObjectWithTag("World").GetComponent<WorldGeneration>().DetectTilePressed(5);
            //}
        }
    }
}
