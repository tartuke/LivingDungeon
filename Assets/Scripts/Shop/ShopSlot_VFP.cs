using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class ShopSlot_VFP : MonoBehaviour
{
    public Image icon;
    public Button button;
    public GoldManager goldManager;

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

    public bool Contains(Item _item)
    {
        return item == _item;
    }

    public Item getItem()
    {
        return item;
    }

    public bool IsEmpty()
    {
        return item == null;
    }

    public void onButtonBuyItem()
    {
        if (goldManager.gold > item.goldCost)
        {
            goldManager.ChangeGold(-item.goldCost);
            Inventory_VFP.instance.Add(item);
            ShopInventory_VFP.instance.Remove(item);
            
        }
        
    }
}
