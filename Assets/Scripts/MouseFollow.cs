using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MouseFollow : MonoBehaviour
{

    public List<DescriptionDisplay> displayShop;
    public List<DescriptionDisplay> displayInventory;
    public List<DescriptionDisplay> displayEquipment;
    public Text text;
    public GameObject panel;
    // Start is called before the first frame update
    void Start()
    {
       
    }

    // Update is called once per frame
    void Update()
    {
        bool isHovering = false;
        Debug.Log("the mouse" + displayShop[0].isHovering);
        for (int i = 0; i < displayShop.Count; i++)
        {
            
            if ((displayShop[i].isHovering) && (displayShop[i].gameObject.GetComponent<ShopSlot_VFP>().getItem() != null))
            {
                panel.SetActive(true);
                Debug.Log("did we make it?");
                text.text = displayShop[i].gameObject.GetComponent<ShopSlot_VFP>().getItem().description;
                panel.transform.position = Input.mousePosition + new Vector3(+160,-70,0);
                isHovering = true;

            }
        }
        for (int i = 0; i < displayInventory.Count; i++)
        {

            if ((displayInventory[i].isHovering) && (displayInventory[i].gameObject.GetComponent<InventorySlot_VFP>().getItem() != null))
            {
                panel.SetActive(true);
                Debug.Log("did we make it?");
                text.text = displayInventory[i].gameObject.GetComponent<InventorySlot_VFP>().getItem().description;
                panel.transform.position = Input.mousePosition + new Vector3(+160, -70, 0);
                isHovering = true;

            }
        }
        for (int i = 0; i < displayEquipment.Count; i++)
        {

            if ((displayEquipment[i].isHovering) && (displayEquipment[i].gameObject.GetComponent<EquipmentUISlots>().getItem() != null))
            {
                panel.SetActive(true);
                Debug.Log("did we make it?");
                text.text = displayEquipment[i].gameObject.GetComponent<EquipmentUISlots>().getItem().description;
                panel.transform.position = Input.mousePosition + new Vector3(+160, -70, 0);
                isHovering = true;

            }
        }
        if (isHovering == false)
        {
            panel.SetActive(false);
        }

        
    }
}
