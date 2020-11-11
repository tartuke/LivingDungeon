using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class InventorySlot : MonoBehaviour
{
    // Start is called before the first frame update
    public Image icon;
    public Button button;

    Item item;

    public GameObject shopUI;

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

    public void onSellButton()
    {
        if (shopUI.activeSelf)
        {
            Inventory.instance.Remove(item);
        }
        
    }
}
