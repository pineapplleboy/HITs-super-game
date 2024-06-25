using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IItemContainer
{
    bool ContainsItem(ItemScriptableObject item);
    void RemoveItem(ItemScriptableObject item);
    bool AddItem(ItemScriptableObject item, int amount);
    bool isFull();
}
