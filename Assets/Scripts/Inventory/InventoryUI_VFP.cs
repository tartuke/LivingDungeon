using UnityEngine;

public class InventoryUI_VFP : MonoBehaviour
{

    public GameObject inventoryUI;
    Inventory_VFP inventory;

    public Transform ItemsParent;
    InventorySlot_VFP[] inventorySlots;

    // Start is called before the first frame update
    void Start()
    {
        inventoryUI.SetActive(false);
        inventory = Inventory_VFP.instance;
        inventory.onItemChangedCallBack += UpdateUI;
        inventorySlots = ItemsParent.GetComponentsInChildren<InventorySlot_VFP>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetButtonDown("Inventory"))
        {
            inventoryUI.SetActive(!inventoryUI.activeSelf);
        }
    }

    void UpdateUI(Item item, InventoryAction action)
    {

        switch (action)
        {
            case InventoryAction.AddItem:
                for (int i = 0; i < inventorySlots.Length; i++)
                {
                    if (inventorySlots[i].IsEmpty())
                    {
                        inventorySlots[i].addItem(item);
                        return;
                    }
                }
                    break;
            case InventoryAction.RemoveItem:
                for (int i = 0; i < inventorySlots.Length; i++)
                {
                    if (inventorySlots[i].Contains(item))
                    {
                        inventorySlots[i].removeItem();
                        return;
                    }
                }
                break;
            default:
                break;
        }
    }
}
