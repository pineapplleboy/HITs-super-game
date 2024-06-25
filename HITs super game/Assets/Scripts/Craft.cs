using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;
using Unity.VisualScripting;
using System;


public class Craft : MonoBehaviour, IPointerDownHandler
{
    public InventorySlot slot;
    private GameObject player;
    public InventoryManager InventoryManager;
    public TMP_Text wallet;
    public ItemScriptableObject item;
    public GameObject Panel;
    public bool isOpened = false;
    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        slot = transform.GetComponentInParent<InventorySlot>();
        Panel.SetActive(true);
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (slot.isEmpty)
            return;
        if (InventoryManager.RemoveItem(item))
        {
            player.GetComponent<PlayerStats>().money += Convert.ToInt32(GetComponentInChildren<TMP_Text>().text);
            wallet.text = player.GetComponent<PlayerStats>().money.ToString();
        }
    }

}
