using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AddShopItems_VFP : MonoBehaviour
{
    ShopInventory_VFP shopInventory;
    public ShopUI_VFP shopUI;

    //maybe make hash list to avoid duplicate items instead
    public List<Item> itemsList = new List<Item>();

    public Item item1;
    public Item item2;
    public Item item3;
    int x = 0;
    // Start is called before the first frame update
    void Start()
    {
        shopInventory = ShopInventory_VFP.instance;

        
    }

    // Update is called once per frame
    void Update()
    {

        if (x == 0)
        {
            shopInventory.Add(item1);
            shopInventory.Add(item2);
            shopInventory.Add(item3);
        }
        x = 1;

    }

    public void newInventory()
    {
        
        //shopInventory.RemoveAll();
        //foreach (Item item in shopInventory.items)
        //{
        //    shopInventory.Remove(item);
        //}
        for (int i = 0; i < shopUI.shopSlots.Length; i++)
        {
            if (!shopUI.shopSlots[i].IsEmpty())
            {
                shopInventory.Remove(shopUI.shopSlots[i].getItem());
            }
        }
        System.Random rand = new System.Random();
        if (itemsList != null)
        {
            for (int i = 0; i < shopUI.shopSlots.Length; i++)
            {
                bool itemIsInChest = false;
                int itemIndex = rand.Next(itemsList.Count);
                Debug.Log("ItemIndex = " + itemIndex);
                if (shopInventory.items.Count == 0)
                {
                    shopInventory.Add(itemsList[itemIndex]);
                }
                else
                {
                    // to avoid duplicate items
                    // currently wont fill chest if there is a duplicate item, itll just go to next part of loop
                    // need fix
                    
                    for (int j = 0; j < shopInventory.items.Count; j++)
                    {
                        
                        if (itemsList[itemIndex] == shopInventory.items[j])
                        {
                            itemIsInChest = true;
                        }
                    
                    }
                    if (!itemIsInChest)
                    {
                        shopInventory.Add(itemsList[itemIndex]);
                    }
                }
            }
        }
        

    }
}
