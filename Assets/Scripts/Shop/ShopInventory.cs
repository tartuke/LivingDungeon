using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShopInventory : MonoBehaviour
{
    public static ShopInventory instance;

    void Awake()
    {
        if (instance != null)
        {
            Debug.LogWarning("More than one instance of Inventory found");
        }
        instance = this;
    }

    public delegate void OnItemChanged();
    public OnItemChanged onItemChangedCallBack;

    public List<Item> items = new List<Item>();

    public void Add(Item item)
    {
        items.Add(item);
        if (onItemChangedCallBack != null)
            onItemChangedCallBack.Invoke(); 
    }

    public void Remove(Item item)
    {
        items.Remove(item);
        if (onItemChangedCallBack != null)
            onItemChangedCallBack.Invoke();
    }
}
