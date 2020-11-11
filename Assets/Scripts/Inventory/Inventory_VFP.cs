using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inventory_VFP : MonoBehaviour
{

    public static Inventory_VFP instance;

    void Awake()
    {
        if (instance != null)
        {
            Debug.LogWarning("More than one instance of Inventory found");
        }
        instance = this;
    }

    public delegate void OnItemChanged(Item item, InventoryAction action);
    public OnItemChanged onItemChangedCallBack;

    public List<Item> items = new List<Item>();

    public void Add (Item item)
    {
        items.Add(item);
        if (onItemChangedCallBack != null)
            onItemChangedCallBack.Invoke(item, InventoryAction.AddItem);
    }

    public void Remove(Item item)
    {
        items.Remove(item);
        if (onItemChangedCallBack != null)
            onItemChangedCallBack.Invoke(item, InventoryAction.RemoveItem);
    }
}

public enum InventoryAction
{
    AddItem,
    RemoveItem
}