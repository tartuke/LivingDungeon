using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class InventorySlot_VFP : MonoBehaviour
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
    public Item getItem()
    {
        return item;
    }

    public bool Contains(Item _item)
    {
        return item == _item;
    }

    public bool IsEmpty()
    {
        return item == null;
    }

    public void onSellButton()
    {
        if (shopUI.activeSelf)
        {
            ShopInventory_VFP.instance.Add(item);
            Inventory_VFP.instance.Remove(item);
        } 
        else
        {
            EquipmentSlot equipmentSlot = item.equipmentSlot;

            Item currentlyEquipped = EquipmentManager.instance.getItem(equipmentSlot);
            EquipmentManager.instance.removeItem(currentlyEquipped);
            EquipmentManager.instance.addItem(item);

            if (equipmentSlot == EquipmentSlot.WEAPON)
            {
                PlayerAction.instance.heldItem = item;
            }
            Inventory_VFP.instance.Remove(item);
            if (!currentlyEquipped.isDefault)
            {
                Inventory_VFP.instance.Add(currentlyEquipped);
            }

            

           
    
        }
        
    }

    public void onDropButton()
    {

        Debug.Log("drop " + item);
        Debug.Log(Inventory_VFP.instance.transform.position);
        item.Drop(Inventory_VFP.instance.transform.position);
        Inventory_VFP.instance.Remove(item);
    }
}
