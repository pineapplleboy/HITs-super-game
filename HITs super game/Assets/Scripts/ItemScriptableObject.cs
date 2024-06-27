using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ItemType { Weapon, Block}
[System.Serializable]
public class ItemScriptableObject : ScriptableObject
{
    public ItemType itemType;
    public GameObject itemPrefab;
    public string itemName;
    public Sprite icon;
    public int maximumAmount;
    public string itemDescription;
    public bool isConsumeable;
}
