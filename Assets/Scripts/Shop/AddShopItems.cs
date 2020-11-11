using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AddShopItems : MonoBehaviour
{
    ShopInventory shopInventory;

    public Item item1;
    public Item item2;
    public Item item3;
    int x = 0;
    // Start is called before the first frame update
    void Start()
    {
        shopInventory = ShopInventory.instance;

        
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
}
