using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;
using System;

public class DragAndDropItem : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IDragHandler
{
    public InventorySlot oldSlot;
    private Transform player;
    public TMP_Text wallet;
    private bool isFacingRight;
    private float throwForce = 10f;
    public GameObject sword;
    public GameObject gun;
    public GameObject laserGun;

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        oldSlot = transform.GetComponentInParent<InventorySlot>();
    }
    public void OnDrag(PointerEventData eventData)
    {
        if (oldSlot.isEmpty)
            return;
        if (oldSlot.isShop)
            return;
        GetComponent<RectTransform>().position += new Vector3(eventData.delta.x, eventData.delta.y);
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (oldSlot.isEmpty)
            return;
        if (oldSlot.isShop)
            return;
        GetComponentInChildren<Image>().color = new Color(1, 1, 1, 0.75f);
        GetComponentInChildren<Image>().raycastTarget = false;
        transform.SetParent(transform.parent.parent);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (oldSlot.isEmpty)
            return;
        if (oldSlot.isShop)
            return;
        GetComponentInChildren<Image>().color = new Color(1, 1, 1, 1);
        GetComponentInChildren<Image>().raycastTarget = true;

        transform.SetParent(oldSlot.transform);
        transform.position = oldSlot.transform.position;
        if (eventData.pointerCurrentRaycast.gameObject.name == "UIPanel")
        {
            if (oldSlot.item.itemName != "kirk")
            {
                DropAndThrowItem();
            }
        }
        else if (eventData.pointerCurrentRaycast.gameObject.transform.parent.parent.GetComponent<InventorySlot>() != null)
        {
            ExchangeSlotData(eventData.pointerCurrentRaycast.gameObject.transform.parent.parent.GetComponent<InventorySlot>());
        }
    }
    public void NullifySlotData()
    {
        oldSlot.item = null;
        oldSlot.amount = 0;
        oldSlot.isEmpty = true;
        oldSlot.iconGO.GetComponent<Image>().color = new Color(0, 0, 0, 255);
        oldSlot.iconGO.GetComponent<Image>().sprite = null;
        oldSlot.itemAmountText.text = "";
    }
    void ExchangeSlotData(InventorySlot newSlot)
    {
        ItemScriptableObject item = newSlot.item;
        int amount = newSlot.amount;
        bool isEmpty = newSlot.isEmpty;
        Sprite tempIcon = newSlot.iconGO.GetComponent<Image>().sprite;
        TMP_Text itemAmountText = newSlot.itemAmountText;
        if (newSlot.isShop == true && oldSlot.item == newSlot.item)
        {
            Debug.Log("shop");
            player.GetComponent<PlayerStats>().money += (Convert.ToInt32(newSlot.amount) * oldSlot.amount);
            Debug.Log(Convert.ToInt32(newSlot.amount));
            Debug.Log(oldSlot.amount);
            wallet.text = player.GetComponent<PlayerStats>().money.ToString();
            oldSlot.item = null;
            oldSlot.amount = 0;
            oldSlot.iconGO.GetComponent<Image>().color = new Color(0, 0, 0, 255);
            oldSlot.iconGO.GetComponent<Image>().sprite = null;
            oldSlot.itemAmountText.text = "";
            oldSlot.isEmpty = true;
        }
        else if (newSlot.isShop == false)
        {
            newSlot.item = oldSlot.item;
            newSlot.amount = oldSlot.amount;
            int maxSum = amount + oldSlot.amount;
            if (oldSlot.isEmpty == false && oldSlot.item != item || oldSlot.isEmpty == false && maxSum >= newSlot.item.maximumAmount || oldSlot.isEmpty == false && maxSum >= oldSlot.item.maximumAmount)
            {
                newSlot.SetIcon(oldSlot.iconGO.GetComponent<Image>().sprite);
                newSlot.itemAmountText.text = oldSlot.amount.ToString();
                newSlot.isEmpty = oldSlot.isEmpty;
            }
            else if (oldSlot.isEmpty == false && oldSlot.item == item)
            {
                newSlot.amount += amount;
                newSlot.itemAmountText.text = newSlot.amount.ToString();
                newSlot.SetIcon(oldSlot.iconGO.GetComponent<Image>().sprite);
                oldSlot.isEmpty = true;
                newSlot.isEmpty = false;
            }
            else
            {
                newSlot.iconGO.GetComponent<Image>().color = new Color(0, 0, 0, 255);
                newSlot.iconGO.GetComponent<Image>().sprite = null;
                newSlot.itemAmountText.text = "";
                newSlot.isEmpty = oldSlot.isEmpty;
            }



            oldSlot.item = item;
            oldSlot.amount = amount;
            if (isEmpty == false && oldSlot.item.itemName != newSlot.item.itemName || isEmpty == false && maxSum >= newSlot.item.maximumAmount/* || isEmpty == false && amount + newSlot.amount >= 64*/)
            {
                oldSlot.SetIcon(tempIcon);
                oldSlot.itemAmountText.text = amount.ToString();
                oldSlot.isEmpty = isEmpty;
            }
            else if (isEmpty == false && oldSlot.item.itemName == newSlot.item.itemName)
            {
                oldSlot.item = null;
                oldSlot.amount = 0;
                oldSlot.iconGO.GetComponent<Image>().color = new Color(0, 0, 0, 255);
                oldSlot.iconGO.GetComponent<Image>().sprite = null;
                oldSlot.itemAmountText.text = "";
                oldSlot.isEmpty = true;
            }
            else
            {
                oldSlot.iconGO.GetComponent<Image>().color = new Color(0, 0, 0, 255);
                oldSlot.iconGO.GetComponent<Image>().sprite = null;
                oldSlot.itemAmountText.text = "";
                oldSlot.isEmpty = isEmpty;
            }
        }
    }
    private void DropAndThrowItem()
    {
        isFacingRight = PlayerMovement.isFacedRight;

        GameObject itemObject = Instantiate(oldSlot.item.itemPrefab, player.position + Vector3.up + player.right * 1.8f, Quaternion.identity);
        if (oldSlot.item.itemType.ToString() == "Weapon")
        {
            SwordAttack.isActive = false;
            Gun.isActive = false;
            LaserGun.isActive = false;
            LaserDamage.isActive = false;
            laserGun.SetActive(false);
            sword.SetActive(false);
            gun.SetActive(false);
        }
        itemObject.GetComponent<Item>().amount = oldSlot.amount;

        Rigidbody2D rb = itemObject.GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            Vector2 throwDirection = new Vector2(isFacingRight ? 1 : -1, 0.5f);
            rb.AddForce(throwDirection * throwForce, ForceMode2D.Impulse);
        }

        NullifySlotData();
    }
}
