using UnityEngine;

public class InventoryUI : MonoBehaviour
{

    public GameObject inventoryUI;
    Inventory inventory;

    public Transform ItemsParent;
    InventorySlot[] inventorySlots;

    // Start is called before the first frame update
    void Start()
    {
        inventoryUI.SetActive(false);
        inventory = Inventory.instance;
        inventory.onItemChangedCallBack += UpdateUI;
        inventorySlots = ItemsParent.GetComponentsInChildren<InventorySlot>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetButtonDown("Inventory"))
        {
            inventoryUI.SetActive(!inventoryUI.activeSelf);
        }
    }

    void UpdateUI()
    {
        for (int i = 0; i < inventorySlots.Length; i++)
        {
            if (i < inventory.items.Count)
            {
                inventorySlots[i].addItem(inventory.items[i]);
            }
            else
            {
                inventorySlots[i].removeItem();
            }
        }
    }
}
