using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class ShopSlot : MonoBehaviour
{
    public Image icon;
    public Button button;

    Item item;


    public void addItem(Item newItem)
    {
        item = newItem;
        icon.sprite = item.icon;
        icon.enabled = true;
    }

    public void removeItem()
    {
        item = null;

        icon.sprite = null;
        icon.enabled = false;

    }

    public void onButtonBuyItem()
    {
        Inventory.instance.Add(item);
        ShopInventory.instance.Remove(item);

        
        
    }
}
