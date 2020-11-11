using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EquipmentUISlots : MonoBehaviour
{
    public EquipmentSlot equipmentSlot;
    public Image icon;
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

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
